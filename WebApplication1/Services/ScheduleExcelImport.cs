using ClosedXML.Excel;
using System.Globalization;
using WebApplication1.ViewModels;

namespace WebApplication1.Services
{
    public record ScheduleImportRow(int RowNumber, ScheduleFormViewModel? Model, string? Error);

    public static class ScheduleExcelImport
    {
        public static List<ScheduleImportRow> Parse(Stream fileStream, IReadOnlyDictionary<string, int> classCodeToId)
        {
            using var wb = new XLWorkbook(fileStream);
            var ws = wb.Worksheets.FirstOrDefault();
            if (ws == null) return new List<ScheduleImportRow> { new(0, null, "Excel file has no worksheet") };

            var headerRow = ws.Row(1);
            var headerMap = BuildHeaderMap(headerRow);

            string[] required =
            [
                "ClassCode",
                "DayOfWeek",
                "Session",
                "Period",
                "StartTime",
                "EndTime",
                "Room",
                "EffectiveDate"
            ];

            var missing = required.Where(r => !headerMap.ContainsKey(r)).ToList();
            if (missing.Count > 0)
            {
                return new List<ScheduleImportRow>
                {
                    new(0, null, $"Missing required columns: {string.Join(", ", missing)}")
                };
            }

            var rows = new List<ScheduleImportRow>();
            var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

            for (int r = 2; r <= lastRow; r++)
            {
                var row = ws.Row(r);
                if (row.IsEmpty()) continue;

                var classCode = GetString(row, headerMap["ClassCode"]);
                if (string.IsNullOrWhiteSpace(classCode))
                {
                    rows.Add(new ScheduleImportRow(r, null, "ClassCode is required"));
                    continue;
                }

                if (!classCodeToId.TryGetValue(classCode.Trim(), out var courseClassId))
                {
                    rows.Add(new ScheduleImportRow(r, null, $"Unknown ClassCode '{classCode}'"));
                    continue;
                }

                if (!TryParseDayOfWeek(GetString(row, headerMap["DayOfWeek"]), out var dayOfWeek))
                {
                    rows.Add(new ScheduleImportRow(r, null, "Invalid DayOfWeek (use Monday..Sunday)"));
                    continue;
                }

                var session = GetString(row, headerMap["Session"]);
                var period = GetString(row, headerMap["Period"]);
                var room = GetString(row, headerMap["Room"]);

                if (string.IsNullOrWhiteSpace(session) || string.IsNullOrWhiteSpace(period) || string.IsNullOrWhiteSpace(room))
                {
                    rows.Add(new ScheduleImportRow(r, null, "Session/Period/Room are required"));
                    continue;
                }

                if (!TryParseTime(row, headerMap["StartTime"], out var startTime))
                {
                    rows.Add(new ScheduleImportRow(r, null, "Invalid StartTime"));
                    continue;
                }

                if (!TryParseTime(row, headerMap["EndTime"], out var endTime))
                {
                    rows.Add(new ScheduleImportRow(r, null, "Invalid EndTime"));
                    continue;
                }

                if (!TryParseDate(row, headerMap["EffectiveDate"], out var effectiveDate))
                {
                    rows.Add(new ScheduleImportRow(r, null, "Invalid EffectiveDate"));
                    continue;
                }

                DateTime? endDate = null;
                if (headerMap.TryGetValue("EndDate", out var endDateCol) &&
                    !IsBlank(row, endDateCol))
                {
                    if (!TryParseDate(row, endDateCol, out var parsedEnd))
                    {
                        rows.Add(new ScheduleImportRow(r, null, "Invalid EndDate"));
                        continue;
                    }
                    endDate = parsedEnd;
                }

                rows.Add(new ScheduleImportRow(
                    r,
                    new ScheduleFormViewModel
                    {
                        CourseClassId = courseClassId,
                        DayOfWeek = dayOfWeek,
                        Session = session.Trim(),
                        Period = period.Trim(),
                        StartTime = startTime,
                        EndTime = endTime,
                        Room = room.Trim(),
                        EffectiveDate = effectiveDate,
                        EndDate = endDate
                    },
                    null));
            }

            return rows;
        }

        private static Dictionary<string, int> BuildHeaderMap(IXLRow headerRow)
        {
            var map = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var cell in headerRow.CellsUsed())
            {
                var name = cell.GetString()?.Trim();
                if (string.IsNullOrWhiteSpace(name)) continue;
                map[name] = cell.Address.ColumnNumber;
            }
            return map;
        }

        private static string GetString(IXLRow row, int col)
        {
            return row.Cell(col).GetString()?.Trim() ?? string.Empty;
        }

        private static bool IsBlank(IXLRow row, int col)
        {
            var cell = row.Cell(col);
            return cell.IsEmpty() || string.IsNullOrWhiteSpace(cell.GetString());
        }

        private static bool TryParseDayOfWeek(string value, out DayOfWeek day)
        {
            day = default;
            if (string.IsNullOrWhiteSpace(value)) return false;

            value = value.Trim();
            if (Enum.TryParse<DayOfWeek>(value, true, out day))
                return true;

            // allow numeric 1-7 (Mon=1)
            if (int.TryParse(value, out var n))
            {
                if (n is >= 1 and <= 7)
                {
                    day = n == 7 ? DayOfWeek.Sunday : (DayOfWeek)n;
                    return true;
                }
            }

            return false;
        }

        private static bool TryParseDate(IXLRow row, int col, out DateTime date)
        {
            date = default;
            var cell = row.Cell(col);

            if (cell.DataType == XLDataType.DateTime)
            {
                date = cell.GetDateTime().Date;
                return true;
            }

            var raw = cell.GetString();
            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsed))
            {
                date = parsed.Date;
                return true;
            }

            return false;
        }

        private static bool TryParseTime(IXLRow row, int col, out string time)
        {
            time = string.Empty;
            var cell = row.Cell(col);

            // If Excel stores time as DateTime
            if (cell.DataType == XLDataType.DateTime)
            {
                time = cell.GetDateTime().ToString("HH:mm", CultureInfo.InvariantCulture);
                return true;
            }

            // Excel classic time can be a number: fraction of a day (e.g., 0.2916667)
            if (cell.DataType == XLDataType.Number)
            {
                var value = cell.GetDouble();
                if (value is >= 0 and < 1)
                {
                    var ts = TimeSpan.FromDays(value);
                    time = new DateTime(1, 1, 1).Add(ts).ToString("HH:mm", CultureInfo.InvariantCulture);
                    return true;
                }
            }

            // String like "7:00:00 AM" or "07:00" etc.
            var raw = cell.GetString()?.Trim();
            if (string.IsNullOrWhiteSpace(raw)) return false;

            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var dt) ||
                DateTime.TryParse(raw, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out dt))
            {
                time = dt.ToString("HH:mm", CultureInfo.InvariantCulture);
                return true;
            }

            if (TimeSpan.TryParse(raw, CultureInfo.InvariantCulture, out var ts2) ||
                TimeSpan.TryParse(raw, CultureInfo.CurrentCulture, out ts2))
            {
                time = new DateTime(1, 1, 1).Add(ts2).ToString("HH:mm", CultureInfo.InvariantCulture);
                return true;
            }

            return false;
        }
    }
}

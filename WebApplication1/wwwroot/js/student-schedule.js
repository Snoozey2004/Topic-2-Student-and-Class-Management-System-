// Student Schedule JavaScript - Modal and Card Selection

// Show class detail modal
function showClassModal(cardElement) {
    // Remove previous selection
    document.querySelectorAll('.class-card').forEach(card => {
        card.classList.remove('selected');
    });
    
    // Mark clicked card as selected
    cardElement.classList.add('selected');
    
    // Get data from card
    const subject = cardElement.getAttribute('data-subject');
    const code = cardElement.getAttribute('data-code');
    const room = cardElement.getAttribute('data-room');
    const lecturer = cardElement.getAttribute('data-lecturer');
    const time = cardElement.getAttribute('data-time');
    
    // Populate modal
    document.getElementById('modalSubject').textContent = subject;
    document.getElementById('modalCode').textContent = code;
    document.getElementById('modalGroup').textContent = code;
    document.getElementById('modalLecturer').textContent = lecturer;
    document.getElementById('modalRoom').textContent = room;
    document.getElementById('modalTime').textContent = time;
    document.getElementById('modalTitle').textContent = subject;
    
    // Show modal
    const modal = document.getElementById('classModal');
    modal.classList.add('active');
    document.body.style.overflow = 'hidden';
}

// Close class detail modal
function closeClassModal() {
    const modal = document.getElementById('classModal');
    modal.classList.remove('active');
    document.body.style.overflow = '';
    
    // Remove selection
    document.querySelectorAll('.class-card').forEach(card => {
        card.classList.remove('selected');
    });
}

// Close modal on ESC key
document.addEventListener('keydown', function(event) {
    if (event.key === 'Escape') {
        closeClassModal();
    }
});

// Prevent modal content click from closing modal
document.addEventListener('DOMContentLoaded', function() {
    const modalContent = document.querySelector('.modal-content-box');
    if (modalContent) {
        modalContent.addEventListener('click', function(event) {
            event.stopPropagation();
        });
    }
});

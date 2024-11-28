document.addEventListener('DOMContentLoaded', function () {
    var deleteTaskModal = document.getElementById('deleteTaskModal');

    deleteTaskModal.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget; 
        var taskId = button.getAttribute('data-task-id'); 
        fetch(`/Task/Delete?id=${taskId}`)
            .then(response => response.text())
            .then(html => {
                document.querySelector('#deleteTaskModal .modal-content').innerHTML = html;
            })
            .catch(error => console.error('Error loading modal:', error));
    });
});
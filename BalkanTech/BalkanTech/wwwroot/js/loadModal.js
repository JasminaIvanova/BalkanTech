document.addEventListener('DOMContentLoaded', function () {
    var deleteTaskModal = document.getElementById('deleteTaskModal');

    deleteTaskModal.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget; // Button that triggered the modal
        var taskId = button.getAttribute('data-task-id'); // Get the task ID

        // Fetch the modal content from the server
        fetch(`/Task/Delete?id=${taskId}`)
            .then(response => response.text())
            .then(html => {
                // Inject the content into the modal's body
                document.querySelector('#deleteTaskModal .modal-content').innerHTML = html;
            })
            .catch(error => console.error('Error loading modal:', error));
    });
});
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


document.addEventListener('DOMContentLoaded', () => {
    const deleteModal = document.getElementById('confirmDeleteModal');
    const deleteForm = document.getElementById('deleteUserForm');
    const userIdInput = document.getElementById('deleteUserId');
    const userFullNameElement = document.getElementById('userFullName');

    deleteModal.addEventListener('show.bs.modal', function (event) {
        const button = event.relatedTarget; 
        const userId = button.getAttribute('data-user-id'); 
        const userName = button.getAttribute('data-user-name');
        userFullNameElement.textContent = userName;
        userIdInput.value = userId;
    });
});

 //document.addEventListener("DOMContentLoaded", function () {
 //       var modalElement = new bootstrap.Modal(document.getElementById('userSelectionModal'));
 //       modalElement.show();
 //   });

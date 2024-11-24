function setCategory(category) {
    document.getElementById("categoryInput").value = category;
    document.getElementById("categoryForm").submit();
}



function changeTaskStatus(taskId, newStatus, newDate) {
    var url = $('#task-status-url').data('url');
    $.post(url, { id: taskId, newStatus: newStatus, newDate: newDate })
        .done(function (response) {
            if (response.success) {
                const taskRow = $('#task-' + taskId);
                $('#task-status-' + taskId).removeClass("bg-warning bg-danger bg-primary bg-success").addClass(response.newStatus === "Completed" ? "bg-success" : "bg-primary").text(response.newStatus);

                if (response.newStatus === "Completed") {
                    taskRow.find('.btn-outline-primary, .btn-outline-success').remove(); 
                    $('#task-date-' + taskId).text(response.newDate);
                    $('#task-passed-' + taskId).remove();
                    $('#completed-tasks-table tbody').append(taskRow);

                    if ($('#completed-tasks-table tbody tr').length > 0) {
                        $('#completed-tasks-table tbody .text-remove').remove();
                    }

                    if ($('#to-be-completed-tasks-table tbody tr').length === 0) {
                        $('#to-be-completed-tasks-table tbody').html('<tr class="text-attach"><td colspan="5" class="text-center">No tasks to be completed.</td></tr>');
                    }
                }
            } else {
                alert("Error: " + response.message);
            }
        })
        .fail(function () {
            alert("An error occurred while changing the task status.");
        });
}



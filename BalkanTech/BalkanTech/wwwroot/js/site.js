function setCategory(category) {
    document.getElementById("categoryInput").value = category;
    document.getElementById("categoryForm").submit();
}

function changeTaskStatus(taskId, newStatus, newDate) {
    var url = $('#task-status-url').data('url');
    $.post(url, { id: taskId, newStatus: newStatus, newDate: newDate })
        .done(function (response) {
            if (response.success) {
                $('#task-status-' + taskId).removeClass("bg-warning bg-danger");
                $('#task-status-' + taskId).text(response.newStatus);

                if (response.newStatus === "In Process") {
                    $('#task-status-' + taskId).addClass("bg-primary");
                } else if (response.newStatus === "Completed") {
                    const task = $('#task-' + taskId).detach();
                    task.find('.btn').remove();
                    $('#completed-tasks-table tbody').append(task);
                    $('#task-status-' + taskId).addClass("bg-success");
                    $('#task-date-' + taskId).text(response.newDate);
                    $('#task-passed-' + taskId).detach();
                    if ($('#completed-tasks-table tbody tr').length > 0) {
                        $('#completed-tasks-table tbody').find('tr.text-remove').remove();
                    }
                    if ($('#to-be-completed-tasks-table tbody tr').length == 0) {
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


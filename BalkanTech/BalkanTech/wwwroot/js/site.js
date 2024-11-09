function setCategory(category) {
    document.getElementById("categoryInput").value = category;
    document.getElementById("categoryForm").submit();
}

function changeTaskStatus(taskId, newStatus) {
    var url = $('#task-status-url').data('url');
    $.post(url, { id: taskId, newStatus: newStatus })
        .done(function (response) {
            if (response.success) {
                $('#task-status-' + taskId).removeClass("bg-warning bg-danger")
                $('#task-status-' + taskId).text(response.newStatus);
                if (response.newStatus === "In Process") {
                    $('#task-status-' + taskId).addClass("bg-danger")
                }
                else if (response.newStatus === "Completed") {
                    const task = $('#task-' + taskId).detach();
                    task.find('.btn').remove();
                    $('#completed-tasks-table tbody').append(task);
                    $('#task-status-' + taskId).addClass("bg-success")
                }
            } else {
                alert("Error: " + response.message);
            }
        })
        .fail(function () {
            alert("An error occurred while changing the task status.");
        });
}
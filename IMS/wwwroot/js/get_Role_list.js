$(document).ready(function () {
    loadRoleList();
})


var dataTable;
function loadRoleList() {
    dataTable = $('#role_table').DataTable({
        "ajax": {
            "url":"/Admin/Role/getRoleList"
        },
        "columns": [
            { "data": "name" },
            { "data": "normalizedName" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="row">
                            <div class="col-6">
                                <a id="${data}" class="btn btn-info" href="/Admin/Role/Upsert?id=${data}">Edit</a>
                            </div>
                            <div class="col-6">
                                <a id="${data}" class="btn btn-danger" onclick="DeleteRole(this.id,event)">Delete</a>
                            </div>
                        <div>
                    `
                }
            }
        ],
        "bDestroy": true,
    });
};
$(document).ready(function () {
    loadUserList();
    showConsole();
})


var dataTable;
function loadUserList() {
    dataTable = $('#user_table').DataTable({
        "ajax": {
            "url": "/User/getUserList"
        },
        "columns": [
            { "data": "fullName" },
            { "data": "email" },
            { "data": "phoneNumber" },
            { "data": "roleId" }
        ],
        "bDestroy": true,
    });
};

//function showConsole() {
//    $.ajax({
//        url:"/User/getUserList",
//        type: "GET",
//        success: function (response) {
//            console.log(response);
//        }
//    });
//}
$(document).ready(function () {
    loadProductList();
})


var dataTable;
function loadProductList() {
    dataTable = $('#product_table').DataTable({
        "ajax": {
            "url":"/Admin/Product/getProductList"
        },
        "columns": [
            { "data": "category.category_Name" },
            { "data": "brand.brand_Name" },
            { "data": "product_Name" },
            { "data": "product_Description" },
            { "data": "product_Price" },
            { "data": "quantity" },
            {
                "data": "inStock",
                "render": function (data) {
                    if (data == true) {
                        return `Yes`;
                    }
                    else {
                        return `No`;
                    }
                }
            },
            {
                "data": "product_Id",
                "render": function (data) {
                    return `
                        <div class="row">
                            <div class="col-6">
                                <a id="${data}" onclick="upsertModal(this.id,event)" class="btn btn-info" href="/Admin/Product/Upsert/${data}">Edit</a>
                            </div>
                            <div class="col-6">
                                <a id="${data}" onclick="DeleteProduct(this.id,event)" class="btn btn-danger" href="/Admin/Product/Delete/${data}">Delete</a>
                            </div>
                        <div>
                    `
                }
            }
        ],
        "bDestroy": true,
    });
};
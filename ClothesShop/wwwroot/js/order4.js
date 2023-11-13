var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#orderTable').DataTable({
        "ajax": { url: '/admin/order/getall' },
        "columns": [
            { "data": "name", "width": "15%" },        
            { "data": "applicationUser.email", "width": "15%" },   
            { "data": "orderStatus", "width": "15%" },
            { "data": "orderTotal", "width": "15%" },  
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                     <a href="/admin/order/Details?orderId=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Details</a>                                 
                    </div>`
                },
                "width": "25%"
            }
        ]
    });
}


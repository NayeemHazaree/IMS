$(document).ready(function () {
    addSerialNumber();
});

//serialize the table
var addSerialNumber = function () {
    var i = 1;
    $('table tr').each(function (index) {
        $(this).find('td:nth-child(1)').html(index - 1 + 1);
    });
};
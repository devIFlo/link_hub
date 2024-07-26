$(document).ready(function () {
    $('#data-table').DataTable();

    $('.btn-service-add').click(function () {
        $.ajax({
            type: 'GET',
            url: '/Links/Create/',
            success: function (result) {
                $("#serviceCreate").html(result);
                $('#modalServicesCreate').modal('show');
            }
        });
    });

    $('.btn-service-edit').click(function () {
        var serviceId = $(this).attr('service-id');

        $.ajax({
            type: 'GET',
            url: '/Links/Edit/' + serviceId,
            success: function (result) {
                $("#serviceEdit").html(result);
                $('#modalServicesEdit').modal('show');
            }
        });
    });

    $('.btn-service-remove').click(function () {
        var serviceId = $(this).attr('service-id');

        $.ajax({
            type: 'GET',
            url: '/Links/Delete/' + serviceId,
            success: function (result) {
                $("#serviceDelete").html(result);
                $('#modalServicesDelete').modal('show');
            }
        });
    });
});

function previewImage(event) {
    var reader = new FileReader();
    reader.onload = function () {
        var output = document.getElementById('imagePreview');
        output.src = reader.result;
        output.style.display = 'block';
    };
    reader.readAsDataURL(event.target.files[0]);
}
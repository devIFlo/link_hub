$(document).ready(function () {
    $('#data-table').DataTable({
        language: {
            url: '/lib/DataTables/pt-BR.json'
        }
    });

    $('#modalPagePermission').on('show.bs.modal', function (event) {
        $('#multiple-select-field').select2({
            dropdownParent: $('#modalPagePermission'),
            theme: "bootstrap-5",
            width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
            placeholder: $(this).data('placeholder')
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


/* Modal da view Links */
$('.btn-link-add').click(function () {
    $.ajax({
        type: 'GET',
        url: '/Links/Create/',
        success: function (result) {
            $("#linkCreate").html(result);
            $('#modalLinkCreate').modal('show');
        }
    });
});

$('.btn-link-edit').click(function () {
    var linkId = $(this).attr('link-id');

    $.ajax({
        type: 'GET',
        url: '/Links/Edit/' + linkId,
        success: function (result) {
            $("#linkEdit").html(result);
            $('#modalLinkEdit').modal('show');
        }
    });
});

$('.btn-link-remove').click(function () {
    var linkId = $(this).attr('link-id');

    $.ajax({
        type: 'GET',
        url: '/Links/Delete/' + linkId,
        success: function (result) {
            $("#linkDelete").html(result);
            $('#modalLinkDelete').modal('show');
        }
    });
});


/* Modal da view Categories */
$('.btn-category-add').click(function () {
    $.ajax({
        type: 'GET',
        url: '/Categories/Create/',
        success: function (result) {
            $("#categoryCreate").html(result);
            $('#modalCategoryCreate').modal('show');
        }
    });
});

$('.btn-category-edit').click(function () {
    var categoryId = $(this).attr('category-id');

    $.ajax({
        type: 'GET',
        url: '/Categories/Edit/' + categoryId,
        success: function (result) {
            $("#categoryEdit").html(result);
            $('#modalCategoryEdit').modal('show');
        }
    });
});

$('.btn-category-remove').click(function () {
    var categoryId = $(this).attr('category-id');

    $.ajax({
        type: 'GET',
        url: '/Categories/Delete/' + categoryId,
        success: function (result) {
            $("#categoryDelete").html(result);
            $('#modalCategoryDelete').modal('show');
        }
    });
});


/* Modal da view Pages */
$('.btn-page-add').click(function () {
    $.ajax({
        type: 'GET',
        url: '/Pages/Create/',
        success: function (result) {
            $("#pageCreate").html(result);
            $('#modalPageCreate').modal('show');
        }
    });
});

$('.btn-page-permission').click(function () {
    var pageId = $(this).attr('page-id');

    $.ajax({
        type: 'GET',
        url: '/Pages/Permission/' + pageId,
        success: function (result) {
            $("#pagePermission").html(result);
            $('#modalPagePermission').modal('show');
        }
    });
});

$('.btn-page-edit').click(function () {
    var pageId = $(this).attr('page-id');

    $.ajax({
        type: 'GET',
        url: '/Pages/Edit/' + pageId,
        success: function (result) {
            $("#pageEdit").html(result);
            $('#modalPageEdit').modal('show');
        }
    });
});

$('.btn-page-remove').click(function () {
    var pageId = $(this).attr('page-id');

    $.ajax({
        type: 'GET',
        url: '/Pages/Delete/' + pageId,
        success: function (result) {
            $("#pageDelete").html(result);
            $('#modalPageDelete').modal('show');
        }
    });
});


/* Modal da view Users */
$('.btn-user-settings').click(function () {
    $.ajax({
        type: 'GET',
        url: '/Users/Settings/',
        success: function (result) {
            $("#userSettings").html(result);
            $('#modalUserSettings').modal('show');
        }
    });
});
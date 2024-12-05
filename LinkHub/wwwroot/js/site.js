const _notyf = new Notyf({
    duration: 10000,
    dismissible: true,
    position: {
        x: 'right',
        y: 'top'
    }
});

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

// Filtro para exibir as categorias de acordo com a página selecionada
function filterCategories() {
    var pageId = document.getElementById("PageId").value;
    var categoryDropdown = document.getElementById("CategoryId");

    if (!pageId) {
        categoryDropdown.disabled = true;
        categoryDropdown.innerHTML = '<option value=""></option>';
        return;
    }

    $.ajax({
        url: '/Links/FilterCategories',
        type: 'GET',
        data: { pageId: pageId },
        success: function (categories) {
            categoryDropdown.disabled = false;
            categoryDropdown.innerHTML = '';
            categoryDropdown.insertAdjacentHTML('beforeend', '<option value=""></option>');

            categories.forEach(function (category) {
                var option = document.createElement("option");
                option.value = category.id;
                option.text = category.name;
                categoryDropdown.appendChild(option);
            });
        }
    });
}


/* Modal da view Links */
$('.btn-link-add').click(function () {
    $.ajax({
        type: 'GET',
        url: '/Links/Create/',
        success: function (result) {
            $("#linkCreate").html(result);
            $('#modalLinkCreate').modal('show');
        },
        error: function (xhr, status, error) {
            _notyf.error('Ocorreu um erro ao tentar abrir a janela de edição.');
        }
    });
});

$('.btn-link-edit').click(function () {
    var linkId = $(this).attr('link-id');

    $.ajax({
        type: 'GET',
        url: '/Links/Edit/' + linkId,
        success: function (result) {
            if (result.message) {
                _notyf.error(result.message);
            } else {
                $("#linkEdit").html(result);
                $('#modalLinkEdit').modal('show');
            }
        },
        error: function (xhr, status, error) {
            _notyf.error('Ocorreu um erro ao tentar abrir a janela de edição.');
        }
    });
});

$('.btn-link-remove').click(function () {
    var linkId = $(this).attr('link-id');

    $.ajax({
        type: 'GET',
        url: '/Links/Delete/' + linkId,
        success: function (result) {
            if (result.message) {
                _notyf.error(result.message);
            } else {
                $("#linkDelete").html(result);
                $('#modalLinkDelete').modal('show');
            }
        },
        error: function (xhr, status, error) {
            _notyf.error('Ocorreu um erro ao tentar abrir a janela de edição.');
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
        },
        error: function (xhr, status, error) {
            _notyf.error('Ocorreu um erro ao tentar abrir a janela de edição.');
        }
    });
});

$('.btn-category-edit').click(function () {
    var categoryId = $(this).attr('category-id');

    $.ajax({
        type: 'GET',
        url: '/Categories/Edit/' + categoryId,
        success: function (result) {
            if (result.message) {
                _notyf.error(result.message);
            } else {
                $("#categoryEdit").html(result);
                $('#modalCategoryEdit').modal('show');
            }
        },
        error: function (xhr, status, error) {
            _notyf.error('Ocorreu um erro ao tentar abrir a janela de edição.');
        }
    });
});

$('.btn-category-remove').click(function () {
    var categoryId = $(this).attr('category-id');

    $.ajax({
        type: 'GET',
        url: '/Categories/Delete/' + categoryId,
        success: function (result) {
            if (result.message) {
                _notyf.error(result.message);
            } else {
                $("#categoryDelete").html(result);
                $('#modalCategoryDelete').modal('show');
            }
        },
        error: function (xhr, status, error) {
            _notyf.error('Ocorreu um erro ao tentar abrir a janela de edição.');
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
        },
        error: function (xhr, status, error) {
            _notyf.error('Ocorreu um erro ao tentar abrir a janela de edição.');
        }
    });
});

$('.btn-page-permission').click(function () {
    var pageId = $(this).attr('page-id');

    $.ajax({
        type: 'GET',
        url: '/Pages/Permission/' + pageId,
        success: function (result) {
            if (result.message) {
                _notyf.error(result.message);
            } else {
                $("#pagePermission").html(result);
                $('#modalPagePermission').modal('show');
            }
        },
        error: function (xhr, status, error) {
            _notyf.error('Ocorreu um erro ao tentar abrir a janela de edição.');
        }
    });
});

$('.btn-page-edit').click(function () {
    var pageId = $(this).attr('page-id');

    $.ajax({
        type: 'GET',
        url: '/Pages/Edit/' + pageId,
        success: function (result) {
            if (result.message) {
                _notyf.error(result.message);
            } else {
                $("#pageEdit").html(result);
                $('#modalPageEdit').modal('show');
            }
        },
        error: function (xhr, status, error) {
            _notyf.error('Ocorreu um erro ao tentar abrir a janela de edição.');
        }
    });
});

$('.btn-page-remove').click(function () {
    var pageId = $(this).attr('page-id');

    $.ajax({
        type: 'GET',
        url: '/Pages/Delete/' + pageId,
        success: function (result) {
            if (result.message) {
                _notyf.error(result.message);
            } else {
                $("#pageDelete").html(result);
                $('#modalPageDelete').modal('show');
            }
        },
        error: function (xhr, status, error) {
            _notyf.error('Ocorreu um erro ao tentar abrir a janela de edição.');
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
        },
        error: function (xhr, status, error) {
            _notyf.error('Ocorreu um erro ao tentar abrir a janela de edição.');
        }
    });
});

$('.btn-user-group').click(function () {
    var userId = $(this).attr('user-id');

    $.ajax({
        type: 'GET',
        url: '/Users/Group/' + userId,
        success: function (result) {
            if (result.message) {
                _notyf.error(result.message);
            } else {
                $("#userGroup").html(result);
                $('#modalUserGroup').modal('show');
            }
        },
        error: function (xhr, status, error) {
            _notyf.error('Ocorreu um erro ao tentar abrir a janela de edição.');
        }
    });
});

$('.btn-user-delete').click(function () {
    var userId = $(this).attr('user-id');

    $.ajax({
        type: 'GET',
        url: '/Users/Delete/' + userId,
        success: function (result) {
            if (result.message) {
                _notyf.error(result.message);
            } else {
                $("#userDelete").html(result);
                $('#modalUserDelete').modal('show');
            }
        },
        error: function (xhr, status, error) {
            _notyf.error('Ocorreu um erro ao tentar abrir a janela de edição.');
        }
    });
});

/* Modal da view Profile */
$('.btn-user-password').click(function () {
    var userId = $(this).attr('user-id');

    $.ajax({
        type: 'GET',
        url: '/Users/Password/' + userId,
        success: function (result) {
            if (result.message) {
                _notyf.error(result.message);
            } else {
                $("#userPassword").html(result);
                $('#modalUserPassword').modal('show');
            }
        },
        error: function (xhr, status, error) {
            _notyf.error('Ocorreu um erro ao tentar abrir a janela de edição.');
        }
    });
});
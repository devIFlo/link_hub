const _notyf = new Notyf({
    duration: 10000,
    dismissible: true,
    position: {
        x: 'right',
        y: 'top'
    }
});

$(document).ready(function () {
    //Configurações do DataTable
    $.fn.dataTable.moment('DD/MM/YYYY');

    // DataTable padrão
    $('#data-table').DataTable({
        language: {
            url: '/lib/DataTables/pt-BR.json'
        }
    });

    // DataTable da página de Logs
    $('#data-table-logs').DataTable({
        language: {
            url: '/lib/DataTables/pt-BR.json'
        },
        order: [[0, 'desc']]
    });

    //Prepara a modal para utilizar o select2
    function initModalSelect(modalId) {
        $(modalId).on('show.bs.modal', function (event) {
            $('#multiple-select-field').select2({
                dropdownParent: $(modalId),
                theme: "bootstrap-5",
                width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
                placeholder: $(this).data('placeholder')
            });
        });
    }

    initModalSelect('#modalPagePermission');
    initModalSelect('#modalLinkHome');
});

// Função para exibir um preview da imagem escolhida
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


// Funções para abertura das modais
function ajaxRequestModal(url, modal, content) {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (result) {
            if (result.message) {
                _notyf.error(result.message);
            } else {
                $("#" + content).html(result);
                $('#' + modal).modal('show');
            }
        },
        error: function () {
            _notyf.error('Ocorreu um erro ao tentar processar a requisição.');
        }
    });
}

function setupModal(btn, controller, action, attrId, modal, content) {
    $(btn).click(function () {
        if (attrId == '') {
            var url = `/${controller}/${action}/`;
        } else {
            var id = $(this).attr(attrId);
            var url = `/${controller}/${action}/${id}`;
        }

        ajaxRequestModal(url, modal, content);
    });
}

// Modais da view Links
setupModal('.btn-link-add', 'Links', 'Create', '', 'modalLinkCreate', 'linkCreate');
setupModal('.btn-link-edit', 'Links', 'Edit', 'link-id', 'modalLinkEdit', 'linkEdit');
setupModal('.btn-link-remove', 'Links', 'Delete', 'link-id', 'modalLinkDelete', 'linkDelete');
setupModal('.btn-link-details', 'Links', 'Details', 'link-id', 'modalLinkDetails', 'linkDetails');

// Modais da view Categories
setupModal('.btn-category-add', 'Categories', 'Create', '', 'modalCategoryCreate', 'categoryCreate');
setupModal('.btn-category-edit', 'Categories', 'Edit', 'category-id', 'modalCategoryEdit', 'categoryEdit');
setupModal('.btn-category-remove', 'Categories', 'Delete', 'category-id', 'modalCategoryDelete', 'categoryDelete');

// Modais da view Pages
setupModal('.btn-page-add', 'Pages', 'Create', '', 'modalPageCreate', 'pageCreate');
setupModal('.btn-page-edit', 'Pages', 'Edit', 'page-id', 'modalPageEdit', 'pageEdit');
setupModal('.btn-page-remove', 'Pages', 'Delete', 'page-id', 'modalPageDelete', 'pageDelete');
setupModal('.btn-page-permission', 'Pages', 'Permission', 'page-id', 'modalPagePermission', 'pagePermission');

// Modais da view Users
setupModal('.btn-user-settings', 'Users', 'Settings', '', 'modalUserSettings', 'userSettings');
setupModal('.btn-user-group', 'Users', 'Group', 'user-id', 'modalUserGroup', 'userGroup');
setupModal('.btn-user-delete', 'Users', 'Delete', 'user-id', 'modalUserDelete', 'userDelete');

// Modais da view Profile
setupModal('.btn-user-password', 'Account', 'ResetPassword', 'user-id', 'modalUserPassword', 'userPassword');


document.addEventListener("DOMContentLoaded", function () {
    // Adiciona evento de clique para o botão de expandir/recolher
    document.querySelectorAll(".btn-toggle-category").forEach(button => {
        button.addEventListener("click", function () {
            const category = this.getAttribute("data-category");
            const rows = document.querySelectorAll(`tr[data-category="${category}"]`);
            const icon = this.querySelector("i");

            rows.forEach(row => {
                row.style.display = row.style.display === "none" ? "" : "none";
            });

            // Alterna o ícone entre "+" e "-"
            if (icon.classList.contains("fa-plus")) {
                icon.classList.remove("fa-plus");
                icon.classList.add("fa-minus");
            } else {
                icon.classList.remove("fa-minus");
                icon.classList.add("fa-plus");
            }
        });
    });
});

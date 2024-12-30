
    //Filtro de busca dos Links exibidos nas HomePages
    function filterLinks() {
        var input = document.getElementById('searchInput');
        var filter = input.value.toLowerCase();
        var tabsContainer = document.getElementById('tabsContainer');
        var tabContents = document.getElementById('tabContents');
        var searchResults = document.getElementById('searchResults');
        var resultsContainer = searchResults.querySelector('.row');
        var allLinkItems = document.querySelectorAll('.link-item');
        var hasVisibleLinks = false;

        resultsContainer.innerHTML = '';

        if (filter === '') {
            tabsContainer.style.display = '';
            tabContents.style.display = '';
            searchResults.style.display = 'none';
            return;
        }

        tabsContainer.style.display = 'none';
        tabContents.style.display = 'none';
        searchResults.style.display = 'block';

        allLinkItems.forEach(function (linkItem) {
            var title = linkItem.querySelector('.card-title').textContent.toLowerCase();
            var description = linkItem.querySelector('.card-text').textContent.toLowerCase();

            if (title.includes(filter) || description.includes(filter)) {
                var alreadyExists = Array.from(resultsContainer.children).some(function (child) {
                    return (
                        child.querySelector('.card-title').textContent === linkItem.querySelector('.card-title').textContent &&
                        child.querySelector('.card-text').textContent === linkItem.querySelector('.card-text').textContent
                    );
                });

                if (!alreadyExists) {
                    var clone = linkItem.cloneNode(true);
                    resultsContainer.appendChild(clone);
                    hasVisibleLinks = true;
                }
            }
        });

        if (!hasVisibleLinks) {
            resultsContainer.innerHTML = '<div class="col text-center">Nenhum link encontrado.</div>';
        }
    }
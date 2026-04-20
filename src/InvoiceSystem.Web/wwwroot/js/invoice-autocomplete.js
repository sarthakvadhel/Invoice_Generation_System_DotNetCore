window.invoiceAutocomplete = {
    init: function (inputId, listId, names) {
        const input = document.getElementById(inputId);
        const list = document.getElementById(listId);

        if (!input || !list) {
            return;
        }

        if (input._autocompleteCleanup) {
            input._autocompleteCleanup();
        }

        const source = Array.isArray(names) ? names.filter(Boolean) : [];
        let selectedIndex = -1;
        let currentSuggestions = [];

        const closeList = () => {
            list.innerHTML = "";
            list.classList.remove("show");
            selectedIndex = -1;
            currentSuggestions = [];
        };

        const render = (items) => {
            list.innerHTML = "";
            currentSuggestions = items;

            if (items.length === 0) {
                list.classList.remove("show");
                return;
            }

            items.forEach((item, index) => {
                const button = document.createElement("button");
                button.type = "button";
                button.className = "autocomplete-item";
                button.textContent = item;
                button.addEventListener("mousedown", (e) => {
                    e.preventDefault();
                    input.value = item;
                    input.dispatchEvent(new Event("input", { bubbles: true }));
                    closeList();
                });

                if (index === selectedIndex) {
                    button.classList.add("active");
                }

                list.appendChild(button);
            });

            list.classList.add("show");
        };

        const getSuggestions = (term) => {
            const q = term.trim().toLowerCase();
            if (!q) {
                return source.slice(0, 8);
            }

            const startsWith = [];
            const contains = [];

            for (const name of source) {
                const value = name.toLowerCase();
                if (value.startsWith(q)) {
                    startsWith.push(name);
                } else if (value.includes(q)) {
                    contains.push(name);
                }
            }

            return [...startsWith, ...contains].slice(0, 8);
        };

        const onInput = () => {
            selectedIndex = -1;
            render(getSuggestions(input.value));
        };

        const onFocus = () => {
            render(getSuggestions(input.value));
        };

        const onKeyDown = (e) => {
            if (!currentSuggestions.length) {
                return;
            }

            if (e.key === "ArrowDown") {
                e.preventDefault();
                selectedIndex = (selectedIndex + 1) % currentSuggestions.length;
                render(currentSuggestions);
            } else if (e.key === "ArrowUp") {
                e.preventDefault();
                selectedIndex = selectedIndex <= 0 ? currentSuggestions.length - 1 : selectedIndex - 1;
                render(currentSuggestions);
            } else if (e.key === "Enter" && selectedIndex >= 0) {
                e.preventDefault();
                const selected = currentSuggestions[selectedIndex];
                input.value = selected;
                input.dispatchEvent(new Event("input", { bubbles: true }));
                closeList();
            } else if (e.key === "Escape") {
                closeList();
            }
        };

        const onDocClick = (e) => {
            if (e.target !== input && !list.contains(e.target)) {
                closeList();
            }
        };

        input.addEventListener("input", onInput);
        input.addEventListener("focus", onFocus);
        input.addEventListener("keydown", onKeyDown);
        document.addEventListener("click", onDocClick);

        input._autocompleteCleanup = () => {
            input.removeEventListener("input", onInput);
            input.removeEventListener("focus", onFocus);
            input.removeEventListener("keydown", onKeyDown);
            document.removeEventListener("click", onDocClick);
            closeList();
        };
    }
};

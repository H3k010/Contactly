// theme
document.addEventListener("DOMContentLoaded", function () {
    const themeRadios = document.querySelectorAll('input[name="theme"]');
    const systemDarkQuery = window.matchMedia('(prefers-color-scheme: dark)');

    function applyTheme(themeValue) {
        document.body.classList.remove('dark-mode');

        if (themeValue === 'dark') {
            document.body.classList.add('dark-mode');
        } else if (themeValue === 'system') {
            if (systemDarkQuery.matches) {
                document.body.classList.add('dark-mode');
            }
        }
    }

    function initializeThemeSelection() {
        const savedTheme = localStorage.getItem('user-theme') || 'system';

        const targetRadio = document.querySelector(`input[name="theme"][value="${savedTheme}"]`);
        if (targetRadio) {
            targetRadio.checked = true;
        }

        applyTheme(savedTheme);
    }

    themeRadios.forEach(radio => {
        radio.addEventListener('change', (e) => {
            const selectedValue = e.target.value;
            localStorage.setItem('user-theme', selectedValue);
            applyTheme(selectedValue);
        });
    });

    systemDarkQuery.addEventListener('change', () => {
        const activeTheme = localStorage.getItem('user-theme') || 'system';
        if (activeTheme === 'system') {
            applyTheme('system');
        }
    });

    initializeThemeSelection();
});

// languages
document.addEventListener('DOMContentLoaded', () => {

    const rootHtml = document.documentElement;
    const langSelect = document.getElementById('langSelect');

    const updateSearchPlaceholder = (selectedLang) => {
        const searchInput = document.getElementById('search');

        if (!searchInput) return;

        const placeholder = searchInput.getAttribute(
            selectedLang === 'ar'
                ? 'data-placeholder-ar'
                : 'data-placeholder-en'
        );

        if (placeholder) {
            searchInput.setAttribute('placeholder', placeholder);
        }
    };

    const updateSearchOptions = (selectedLang) => {
        const searchBy = document.getElementById('searchBy');

        if (!searchBy) return;

        searchBy.querySelectorAll('option').forEach(option => {
            const text = option.getAttribute(
                selectedLang === 'ar'
                    ? 'data-ar'
                    : 'data-en'
            );

            if (text) {
                option.textContent = text;
            }
        });
    };

    const syncFormInputs = (selectedLang) => {
        document.querySelectorAll('form').forEach(form => {
            const enInput = form.querySelector('[lang="en"]');
            const arInput = form.querySelector('[lang="ar"]');

            if (enInput && arInput) {
                enInput.disabled = selectedLang === 'ar';
                arInput.disabled = selectedLang !== 'ar';
            }
        });
    };

    const initialLang =
        rootHtml.getAttribute('data-current-lang') || 'en';


    updateSearchPlaceholder(initialLang);
    updateSearchOptions(initialLang);
    syncFormInputs(initialLang);

    if (!langSelect) return;

    langSelect.value = initialLang;

    langSelect.addEventListener('change', (event) => {
        const selectedLang = event.target.value;

        rootHtml.setAttribute('data-current-lang', selectedLang);
        localStorage.setItem('user-language', selectedLang);

        if (selectedLang === 'ar') {
            rootHtml.setAttribute('dir', 'rtl');
            rootHtml.setAttribute('lang', 'ar');
        } else {
            rootHtml.setAttribute('dir', 'ltr');
            rootHtml.setAttribute('lang', 'en');
        }

        document.cookie = `.AspNetCore.Culture=c=${selectedLang}|uic=${selectedLang};path=/;max-age=31536000`;
        window.location.reload();
    });
});


// modal
let pendingSensitiveForm = null;
let pendingSensitiveUrl = null;

function submitSensitiveForm(button) {

    const form = button.closest("form");

    if (form && !$(form).valid()) {
        return;
    }

    pendingSensitiveForm = form;
    pendingSensitiveUrl = button.dataset.url || null;

    openConfirmPasswordModal(
        button.dataset.action,
        button.dataset.title,
        button.dataset.message,
        button.dataset.submitText
    );
}

function openConfirmPasswordModal(action, title, message, submitText) {

    document.getElementById("sensitiveAction").value = action;

    document.getElementById("confirmPasswordTitle").textContent = title;

    document.getElementById("confirmPasswordMessage").textContent = message;

    document.getElementById("confirmPasswordSubmit").textContent = submitText;

    document.getElementById("confirmPasswordError").textContent = "";

    document.getElementById("confirmPasswordModal").classList.add("show");

    document.getElementById("Password").focus();
}

function closeConfirmPasswordModal() {

    const modal = document.getElementById("confirmPasswordModal");

    modal.classList.remove("show");

    document.getElementById("Password").value = "";
}

const confirmPasswordForm = document.getElementById("confirmPasswordForm");

if (confirmPasswordForm) {

    confirmPasswordForm.addEventListener("submit", async function (event) {

        event.preventDefault();

        const form = event.target;

        if (!$(form).valid()) {
            return;
        }

        const response = await fetch(form.action, {
            method: "POST",
            body: new FormData(form)
        });

        const result = await response.json();

        if (!result.success) {

            document.getElementById("confirmPasswordError").textContent = result.message;

            return;
        }

        closeConfirmPasswordModal();

        if (pendingSensitiveForm) {

            pendingSensitiveForm.requestSubmit();

        } else if (pendingSensitiveUrl) {

            window.location.href = pendingSensitiveUrl;
        }

        pendingSensitiveForm = null;
        pendingSensitiveUrl = null;
    });
}

/* Linked select options */
document.getElementById("categorySelect").addEventListener("change", function () {
    var selectedPrefix = this.value;
    var subcategorySelect = document.getElementById("subcategorySelect");

    Array.from(subcategorySelect.options).forEach(opt => {
        opt.hidden = opt.value !== "" && !opt.dataset.cat?.startsWith(selectedPrefix);
    });

    subcategorySelect.value = "";
});

/* Toggler for the add menu option */
function toggleAddOptionInput() {
    const container = document.getElementById("newOptionContainer");
    const button = document.getElementById("toggleOptionInputBtn")
    const icon = document.getElementById("toggleOptionIcon");
    const isHidden = container.classList.contains("d-none");

    if (isHidden) {
        container.classList.remove("d-none");
        button.classList.remove("btn-secondary");
        button.classList.add("btn-danger");
        icon.classList.remove("bi-plus-square");
        icon.classList.add("bi-x-square");
        document.getElementById("newOptionInput").focus();
    } else {
        container.classList.add("d-none");
        button.classList.remove("btn-danger");
        button.classList.add("btn-secondary");
        icon.classList.remove("bi-x-square");
        icon.classList.add("bi-plus-square");
        document.getElementById("newOptionInput").value = "";
    }
}

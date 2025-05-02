
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

/* Add menu option function */
function addMenuOption() {
    let index = document.querySelectorAll(".menu-list details").length;
    let name = document.getElementById("newOptionInput").value.trim();
    let order = document.getElementById("newOptionOrder").value.trim();

    if (name === "" || order === "") {
        alert("Please enter a valid name and order!");
        return;
    }

    // Create new list item
    const li = document.createElement("li");
    const details = document.createElement("details");

    // Create summary for collapsible effect
    const summary = document.createElement("summary");
    summary.innerHTML = `<strong>📁 ${name} - ${order}</strong>`;

    const hiddenId = document.createElement("input");
    hiddenId.type = "hidden";
    hiddenId.name = `Options[${index}].Id`;
    hiddenId.value = "0"; // Let EF Core handle ID assignment

    const hiddenName = document.createElement("input");
    hiddenName.type = "hidden";
    hiddenName.name = `Options[${index}].Name`;
    hiddenName.value = name;

    const hiddenOrder = document.createElement("input");
    hiddenOrder.type = "hidden";
    hiddenOrder.name = `Options[${index}].Order`;
    hiddenOrder.value = order;

    // Append elements correctly
    details.appendChild(summary);
    details.appendChild(hiddenId);
    details.appendChild(hiddenName);
    details.appendChild(hiddenOrder);
    li.appendChild(details);

    // Add to menu list
    document.querySelector(".menu-list").appendChild(li);

    // Clear input fields
    document.getElementById("newOptionInput").value = "";
    document.getElementById("newOptionOrder").value = "";
}

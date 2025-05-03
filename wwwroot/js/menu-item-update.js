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

    // ✅ Update the dropdown with the new option
    const newOption = document.createElement("option");
    newOption.value = index;
    newOption.textContent = name;
    document.getElementById("optionSelect").appendChild(newOption);

    // Clear input fields
    document.getElementById("newOptionInput").value = "";
    document.getElementById("newOptionOrder").value = "";

    document.getElementById("no-options").style.display = "none";
}

/* Add subgroup to option */
function addSubcategoryToSelectedOption() {
    const selectedOptionIndex = document.getElementById("optionSelect").value;
    const subgroupSelect = document.getElementById("subcategorySelect");
    const selectedSubgroupValue = subgroupSelect.value;
    const selectedSubgroupText = subgroupSelect.options[subgroupSelect.selectedIndex].text;

    if (selectedOptionIndex === "" || selectedSubgroupValue === "") {
        alert("Моля, изберете опция и подгрупа!");
        return;
    }

    const menuListItems = document.querySelectorAll(".menu-list > li");
    const selectedLi = menuListItems[selectedOptionIndex];
    if (!selectedLi) {
        alert("Избраната опция не съществува в списъка.");
        return;
    }

    const details = selectedLi.querySelector("details");

    // Try to find an existing child list
    let childList = details.querySelector("ul.child-list");

    // If no child list exists, create it and insert before the fallback "Няма добавени подопции" paragraph
    if (!childList) {
        childList = document.createElement("ul");
        childList.className = "child-list";

        const fallbackParagraph = details.querySelector(`#no-option-children-${selectedOptionIndex}`);
        if (fallbackParagraph) {
            fallbackParagraph.remove(); // Remove the "no options" message
        }

        details.appendChild(childList);
    }

    // Calculate the new child index
    const childCount = childList.querySelectorAll("li").length;

    // Create new list item
    const li = document.createElement("li");
    li.className = "list-group-item";
    li.innerHTML = `
        <span>📌 ${selectedSubgroupText}</span>
        <input type="hidden" name="Options[${selectedOptionIndex}].Children[${childCount}].Id" value="0"/>
        <input type="hidden" name="Options[${selectedOptionIndex}].Children[${childCount}].ReferenceId" value="${selectedSubgroupValue}"/>
        <input type="hidden" name="Options[${selectedOptionIndex}].Children[${childCount}].DisplayName" value="${selectedSubgroupText}"/>
        <input type="hidden" name="Options[${selectedOptionIndex}].Children[${childCount}].Type" value="SubGroup"/>
    `;

    // Append to the list
    childList.appendChild(li);
}

/* Toggler for the add menu option set */
function toggleAddOptionSetInput() {
    const container = document.getElementById("newSetContainer");
    const button = document.getElementById("toggleOptionSetInputBtn")
    const isHidden = container.classList.contains("d-none");

    if (isHidden) {
        container.classList.remove("d-none");
        button.classList.remove("btn-secondary");
        button.classList.add("btn-danger");
        document.getElementById("newSetInput").focus();
    } else {
        container.classList.add("d-none");
        button.classList.remove("btn-danger");
        button.classList.add("btn-secondary");
        document.getElementById("newSetInput").value = "";
    }
}

/* Add menu option set */
function addMenuOptionSet() {
    const selectedOptionIndex = document.getElementById("optionSelect").value;
    const setName = document.getElementById("newSetInput").value.trim();

    if (selectedOptionIndex === "" || setName === "") {
        alert("Please select an option and enter a valid set name!");
        return;
    }

    const menuListItems = document.querySelectorAll(".menu-list > li");
    const selectedLi = menuListItems[selectedOptionIndex];
    if (!selectedLi) {
        alert("Selected option index is invalid.");
        return;
    }

    const details = selectedLi.querySelector("details");

    // Try to find an existing child list
    let childList = details.querySelector("ul.child-list");
    if (!childList) {
        childList = document.createElement("ul");
        childList.className = "child-list";

        const fallbackParagraph = details.querySelector(`#no-option-children-${selectedOptionIndex}`);
        if (fallbackParagraph) {
            fallbackParagraph.remove(); // Remove "no children" message
        }

        details.appendChild(childList);
    }

    // Determine new child index
    const existingChildren = childList.querySelectorAll("li, details");
    const childIndex = existingChildren.length;

    // Create new details block for the set
    const setDetails = document.createElement("details");
    setDetails.innerHTML = `
        <summary>
        <strong>📂 ${setName}</strong>
        <button type="button" class="btn btn-sm btn-outline-primary mt-2" onclick="addSubgroupToThisSet(this)">Добави в сета</button>
        </summary>
        <ul class="set-list"></ul>
        <input type="hidden" name="Options[${selectedOptionIndex}].Children[${childIndex}].DisplayName" value="${setName}"/>
        <input type="hidden" name="Options[${selectedOptionIndex}].Children[${childIndex}].Type" value="SubGroupSet"/>
    `;

    // Append the new set to the child list
    childList.appendChild(setDetails);

    // Clear the input
    document.getElementById("newSetInput").value = "";
}

function addSubgroupToThisSet(button) {
    const subgroupSelect = document.getElementById("subcategorySelect");
    const subgroupValue = subgroupSelect.value;
    const subgroupText = subgroupSelect.options[subgroupSelect.selectedIndex]?.text;

    if (!subgroupValue) {
        alert("Моля, изберете подгрупа за добавяне.");
        return;
    }

    const setDetails = button.closest("details");
    const setList = setDetails.querySelector(".set-list");

    const optionLi = button.closest("li");
    const optionIndex = Array.from(document.querySelectorAll(".menu-list > li")).indexOf(optionLi);
    if (optionIndex === -1) {
        alert("Не може да се определи опцията.");
        return;
    }

    const childList = optionLi.querySelector(".child-list");

    // Compute the model-aligned Children index (j) of the current SubGroupSet
    let setIndex = -1;
    let currentIndex = 0;
    for (const child of childList.children) {
        if (child.tagName === "LI") {
            // SubGroup – skip
            currentIndex++;
        } else if (child.tagName === "DETAILS") {
            // SubGroupSet – check if it's the one we're inside
            if (child === setDetails) {
                setIndex = currentIndex;
                break;
            }
            currentIndex++;
        }
    }

    if (setIndex === -1) {
        alert("Не може да се определи индекс на сета.");
        return;
    }

    const itemCount = setList.querySelectorAll("li").length;

    const li = document.createElement("li");
    li.innerHTML = `
        <span>🔹 ${subgroupText}</span>
        <input type="hidden" name="Options[${optionIndex}].Children[${setIndex}].SubGroupSetItems[${itemCount}].Id" value="${subgroupValue}"/>
        <input type="hidden" name="Options[${optionIndex}].Children[${setIndex}].SubGroupSetItems[${itemCount}].Alias" value="${subgroupText}"/>
    `;

    setList.appendChild(li);
}




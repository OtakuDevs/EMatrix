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
document.querySelector("details").addEventListener("toggle", function () {
    const icon = this.querySelector("summary i");
    if (this.open) {
        icon.classList.remove("bi-plus-square");
        icon.classList.add("bi-x-square");
    } else {
        icon.classList.remove("bi-x-square");
        icon.classList.add("bi-plus-square");
    }
});


/* Add menu option function */
function addMenuOption() {
    let index = document.querySelectorAll(".menu-list > li > details").length;
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
    summary.innerHTML = `
            <strong>📁 ${name} - ${order}</strong>
             <button type="button" class="btn btn-outline-secondary btn-sm" onclick="removeMenuOption(this)">X</button>
            `;

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

    // Add this to the end of your addMenuOption() function:
    optionsData.push({
        id: 0,
        name: name,
        order: order,
        children: []
    });

    // Clear input fields
    document.getElementById("newOptionInput").value = "";
    document.getElementById("newOptionOrder").value = "";

    const noOption = document.getElementById("no-options");
    if(noOption !== null) {
        noOption.style.display = "none";
    }
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
        <span>
        📌 ${selectedSubgroupText}
        <button type="button" class="btn btn-outline-secondary btn-sm" onclick="removeMenuOptionSubgroup(this)">X</button>
        </span>
        <input type="hidden" name="Options[${selectedOptionIndex}].Children[${childCount}].Id" value="0"/>
        <input type="hidden" name="Options[${selectedOptionIndex}].Children[${childCount}].ReferenceId" value="${selectedSubgroupValue}"/>
        <input type="hidden" name="Options[${selectedOptionIndex}].Children[${childCount}].DisplayName" value="${selectedSubgroupText}"/>
        <input type="hidden" name="Options[${selectedOptionIndex}].Children[${childCount}].Type" value="SubGroup"/>
    `;

    // Append to the list
    childList.appendChild(li);
    details.open = true;
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
        <div class="d-flex flex-row gap-2">
        <button type="button" class="btn btn-sm btn-main" onclick="addSubgroupToThisSet(this)">Добави подгрупа към комплект</button>
        <button type="button" class="btn btn-outline-secondary btn-sm" onclick="removeMenuOptionSet(this)">X</button>
        </div>
        </summary>
        <ul class="set-list"></ul>
        <input type="hidden" name="Options[${selectedOptionIndex}].Children[${childIndex}].DisplayName" value="${setName}"/>
        <input type="hidden" name="Options[${selectedOptionIndex}].Children[${childIndex}].Type" value="SubGroupSet"/>
    `;

    // Append the new set to the child list
    childList.appendChild(setDetails);

    // Clear the input
    document.getElementById("newSetInput").value = "";
    details.open = true;
}

/* Add subgroup to Menu Option Set */
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
        <span>
        🔹 ${subgroupText}
        <button type="button" class="btn btn-outline-secondary btn-sm" onclick="removeMenuOptionSetSubgroup(this)">X</button>
        </span>
        <input type="hidden" name="Options[${optionIndex}].Children[${setIndex}].SubGroupSetItems[${itemCount}].Id" value="${subgroupValue}"/>
        <input type="hidden" name="Options[${optionIndex}].Children[${setIndex}].SubGroupSetItems[${itemCount}].Alias" value="${subgroupText}"/>
    `;

    setList.appendChild(li);
    setDetails.open = true;
}

/* Remove Menu Option */
function removeMenuOption(button) {
    const li = button.closest("li");
    if (!li) {
        alert("Не можа да се намери родителският елемент за изтриване.");
        return;
    }

    // Find the index of the <li> within the top-level list
    const listItems = Array.from(document.querySelectorAll(".menu-list > li"));
    const index = listItems.indexOf(li);

    // Remove the corresponding option from the select
    if (index >= 0) {
        const select = document.getElementById("optionSelect");
        select.remove(index + 1); // +1 to skip the placeholder option (like "-- Избери опция --")
    }

    // Optional: Remove from optionsData if maintained
    if (Array.isArray(window.optionsData)) {
        optionsData.splice(index, 1);
    }

    li.remove();
}


function removeMenuOptionSubgroup(button) {
    const li = button.closest("li");
    if (!li) {
        alert("Неуспешно");
        return;
    }
    li.remove();
}

function removeMenuOptionSet(button) {
    const details = button.closest("details");
    if (!details) {
        alert("Неуспешно");
        return;
    }
    details.remove();
}

function removeMenuOptionSetSubgroup(button) {
    const li = button.closest("li");
    if (!li) {
        alert("Неуспешно");
        return;
    }
    li.remove();
}

/* Edit menu option */
document.getElementById("optionSelect").addEventListener("change", function () {
    const selectedIndex = this.value;
    const option = optionsData[selectedIndex]; // We'll define `optionsData` below
    console.log(option);

    if (option) {
        document.getElementById("editOptionInput").value = option.name;
        document.getElementById("editOptionOrder").value = option.order;
    }
});

function updateSelectedOption() {
    const select = document.getElementById("optionSelect");
    const selectedIndex = select.selectedIndex;
    const newName = document.getElementById("editOptionInput").value;
    const newOrder = document.getElementById("editOptionOrder").value;

    if (selectedIndex <= 0) {
        alert("Моля, изберете опция за редактиране.");
        return;
    }

    const optionIndex = select.value; // This is the index from model (used in input names)

    // Update hidden model inputs
    document.querySelector(`input[name="Options[${optionIndex}].Name"]`).value = newName;
    document.querySelector(`input[name="Options[${optionIndex}].Order"]`).value = newOrder;

    // Update preview summary
    const previewListItems = document.querySelectorAll(".menu-list > li");
    const targetDetails = previewListItems[optionIndex]?.querySelector("details");

    if (targetDetails) {
        const summary = targetDetails.querySelector("summary > strong");
        if (summary) {
            summary.textContent = `📁 ${newName} - ${newOrder}`;
        }
    }

    // Update select text
    select.options[selectedIndex].text = newName;
}






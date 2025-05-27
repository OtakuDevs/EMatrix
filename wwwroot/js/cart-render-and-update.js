
let cart = JSON.parse(localStorage.getItem('cart')) || [];
updateCartUI(); // render on load if there are items

function addProductToCart(button) {
    const card = button.closest('.product-card');

    const idText = card.querySelector('.product-code')?.textContent.trim();
    const id = idText?.split(":")[1]?.trim();
    const name = card.querySelector('.product-title')?.textContent.trim();
    const subgroup = card.querySelector('.product-subcategory')?.textContent.trim();
    const icon = card.querySelector('img').getAttribute('src');
    const priceText = card.querySelector('.product-price')?.textContent.trim();
    const price = parseFloat(priceText?.split(":")[1]?.replace("лв.", "").trim().replace(',', '.')) || 0;

    // Check if product already in cart
    const existing = cart.find(p => p.id === id);
    if (existing) {
        existing.qty += 1;
    } else {
        cart.push({id, name, subgroup, icon, price, qty: 1});
    }
    localStorage.setItem('cart', JSON.stringify(cart));
    updateCartUI();
}

function addProductToCartFromDetails(button) {
    const card = button.closest('.product-details-card');

    const idText = card.querySelector('.product-code')?.textContent.trim();
    const id = idText?.split(":")[1]?.trim();
    const name = card.querySelector('.product-title')?.textContent.trim();
    let subgroup = card.querySelector('.item-subcategory')?.textContent.trim();
    if(subgroup === undefined) {
        subgroup = "";
    }
    const icon = card.querySelector('img').getAttribute('src');
    const priceText = card.querySelector('.product-price')?.textContent.trim();
    const price = parseFloat(priceText?.replace("лв.", "").trim().replace(',', '.')) || 0;

    const existing = cart.find(p => p.id === id);
    if (existing) {
        existing.qty += 1;
    } else {
        cart.push({id, name, subgroup, icon, price, qty: 1});
    }
    localStorage.setItem('cart', JSON.stringify(cart));
    updateCartUI();
}

function changeQty(id, delta) {
    const item = cart.find(p => p.id === id);
    if (!item) return;

    item.qty += delta;

    if (item.qty < 1) {
        cart = cart.filter(p => p.id !== id);
    }
    localStorage.setItem('cart', JSON.stringify(cart));
    updateCartUI(); // re-render cart
}

function updateCartUI() {
    const cartContent = document.querySelector("#cart-content");
    const cartTotal = document.querySelector("#cart-total");
    const cartItems = document.querySelector("#cart-items");

    cartContent.innerHTML = ""; // Clear all content

    let total = 0;
    let totalItems = 0;

    // Add Header always
    const header = document.createElement("div");
    header.classList.add("cart-content-header");
    header.innerHTML = `
        <div class="cart-header-item">Артикул</div>
        <div class="cart-header-qty">Количество</div>
        <div class="cart-header-price">Цена</div>
    `;
    cartContent.appendChild(header);

    // Create list container
    const ul = document.createElement("ul");

    if (cart.length === 0) {
        const li = document.createElement("li");
        li.innerHTML = `<p class="text-center cart-empty">Вашата количка е празна!</p>`;
        ul.appendChild(li);

        cartItems.textContent = "0";
        cartItems.classList.add("count-zero");
        cartTotal.textContent = "0 продукт(а) - 0.00лв.";
    } else {
        cart.forEach(item => {
            total += item.price * item.qty;
            totalItems += item.qty;

            const li = document.createElement("li");
            li.innerHTML = `
                <div class="cart-item">
                    <img src="${item.icon}" alt="${item.name}">
                    <div class="cart-item-names">
                        <p class="m-0 cart-item-name">${item.name}</p>
                        <p class="m-0 cart-item-subgroup">${item.subgroup}</p>
                    </div>
                    <div class="cart-item-qty-control">
                        <button class="qty-btn" onclick="changeQty('${item.id}', -1)">−</button>
                        <span class="qty-display">${item.qty}</span>
                        <button class="qty-btn" onclick="changeQty('${item.id}', 1)">+</button>
                    </div>
                    <p class="cart-item-price">${item.price.toFixed(2)} лв.</p>
                </div>
            `;
            ul.appendChild(li);
        });

        cartItems.textContent = totalItems;
        cartItems.classList.remove("count-zero");
        cartTotal.textContent = `${totalItems} продукт(а) - ${total.toFixed(2)}лв.`;
    }

    cartContent.appendChild(ul);

    // Always show footer
    const footer = document.createElement("div");
    footer.classList.add("cart-content-footer");

// Use flex to align total and button
    footer.innerHTML = `
    <div style="display: flex; justify-content: space-between; align-items: center; width: 100%;">
        <a id="review-btn" class="btn btn-main btn-sm text-nowrap">Преглед</a>
        <div>Общо: <span id="cart-footer-total">${total.toFixed(2)} лв.</span></div>
    </div>
`;

    cartContent.appendChild(footer);
}

document.getElementById("review-btn").addEventListener("click", () => {
    fetch('/Cart/ReceiveCartContent', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(cart)
    })
        .then(response => {
            if (response.ok) {
                window.location.href = response.url.replace(/ReceiveCartContent$/, "CartPreview");
            } else {
                return response.text().then(text => console.error("Response:", text));
            }
        })
        .catch(error => console.error("Error:", error));
});


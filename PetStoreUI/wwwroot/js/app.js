// wwwroot/js/app.js

async function loadFeatures(petType) {
    try {
        const response = await fetch(`/Home/GetFeatures?petType=${petType}`);
        const features = await response.json();
        const productList = document.getElementById('product-list');
        productList.innerHTML = ''; // Xóa danh sách cũ

        if (features && features.length > 0) {
            features.forEach(feature => {
                const product = document.createElement('div');
                product.classList.add('product-item');
                product.innerHTML = `
                    <img src="${feature.ImageUrl}" alt="${feature.PetDescription}" class="product-image">
                    <h3>${feature.PetDescription}</h3>
                    <p>Price: ${feature.Price} VND</p>
                    <p>${feature.Specifications}</p>
                    <button class="btn add-to-cart">Add to Cart</button>
                    <button class="btn details">Details</button>
                `;
                productList.appendChild(product);
            });
        } else {
            productList.innerHTML = '<p>No features available for this pet type.</p>';
        }
    } catch (error) {
        console.error('Error loading features:', error);
    }
}

window.renderOrderStatusChart = (data) => {
    const labels = data.map(x => x.label);
    const values = data.map(x => x.count);

    const ctx = document.getElementById('orderStatusChart').getContext('2d');

    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{ data: values }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false
        }
    });
};

window.renderPaymentChart = (online, cod) => {
    const ctx = document.getElementById('paymentChart').getContext('2d');

    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: ['Online', 'Cash On Delivery'],
            datasets: [{ data: [online, cod] }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false
        }
    });
};

window.renderProductChart = (active, inactive) => {
    const ctx = document.getElementById('productChart').getContext('2d');

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Active Products', 'Inactive Products'],
            datasets: [{ data: [active, inactive] }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false
        }
    });
};

let existingChart = null;

window.createOrUpdatePieChart = function (dataJson) {
    const data = JSON.parse(dataJson);
    console.log('Data received:', data);

    const canvas = document.getElementById('myPieChart');
    if (!canvas) {
        console.error('Canvas element with id "myPieChart" not found.');
        return;
    }

    const ctx = canvas.getContext('2d');
    if (!ctx) {
        console.error('Failed to get 2D rendering context for the canvas.');
        return;
    }

    // Destroy the existing chart instance if it exists
    if (existingChart) {
        existingChart.destroy();
    }

    const labels = data.map(item => `${item.label} (${item.value})`);
    const values = data.map(item => item.value);

    existingChart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: values,
                backgroundColor: [
                    '#A6D59B', // Green for 'For'
                    '#FD7764', // Red for 'Against'
                    '#F2DD81'  // Yellow for 'Avoid'
                ],
                hoverBackgroundColor: [
                    '#A6D59B', // Green for 'For'
                    '#FD7764', // Red for 'Against'
                    '#F2DD81'  // Yellow for 'Avoid'
                ]
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: true, // Show the legend
                    position: 'bottom', // Position the legend at the bottom of the chart
                    labels: {
                        usePointStyle: true // Use the same color and shape for the legend labels
                    }
                }
            },
            datalabels: {
                display: true,
                formatter: (value, context) => {
                    return context.chart.data.labels[context.dataIndex];
                }
            }
        }
    });



};
const rowsPerPage = 10;
let currentPage = 1;

const table = document.getElementById("usersTable");
const tbody = document.getElementById("usersTableBody");
const rows = Array.from(tbody.querySelectorAll("tr"));

function renderTable(page) {
    const start = (page - 1) * rowsPerPage;
    const end = start + rowsPerPage;

    rows.forEach((row, index) => {
        row.style.display = (index >= start && index < end) ? "" : "none";
    });

    document.getElementById("pageInfo").textContent =
        `Page ${page} of ${Math.ceil(rows.length / rowsPerPage)}`;
    document.getElementById("prevPage").disabled = page === 1;
    document.getElementById("nextPage").disabled = end >= rows.length;
}

function applyFilters() {
    const nameFilter = document.getElementById("searchByName").value.toLowerCase();
    const emailFilter = document.getElementById("searchByEmail").value.toLowerCase();
    const roleFilter = document.getElementById("searchByRole").value.toLowerCase();

    rows.forEach(row => {
        const name = row.cells[1].textContent.toLowerCase();
        const email = row.cells[2].textContent.toLowerCase();
        const role = row.cells[3].textContent.toLowerCase();

        const matches = name.includes(nameFilter) &&
            email.includes(emailFilter) &&
            role.includes(roleFilter);

        row.style.display = matches ? "" : "none";
    });

    // Recalculate pagination after filtering
    currentPage = 1;
    paginateAfterFilter();
}

function paginateAfterFilter() {
    const visibleRows = rows.filter(r => r.style.display !== "none");
    visibleRows.forEach((row, index) => {
        row.style.display = (index >= (currentPage - 1) * rowsPerPage &&
            index < currentPage * rowsPerPage) ? "" : "none";
    });

    document.getElementById("pageInfo").textContent =
        `Page ${currentPage} of ${Math.ceil(visibleRows.length / rowsPerPage)}`;
    document.getElementById("prevPage").disabled = currentPage === 1;
    document.getElementById("nextPage").disabled = currentPage * rowsPerPage >= visibleRows.length;
}

document.getElementById("prevPage").addEventListener("click", () => {
    if (currentPage > 1) {
        currentPage--;
        paginateAfterFilter();
    }
});

document.getElementById("nextPage").addEventListener("click", () => {
    const visibleRows = rows.filter(r => r.style.display !== "none");
    if (currentPage * rowsPerPage < visibleRows.length) {
        currentPage++;
        paginateAfterFilter();
    }
});

document.getElementById("searchByName").addEventListener("input", applyFilters);
document.getElementById("searchByEmail").addEventListener("input", applyFilters);
document.getElementById("searchByRole").addEventListener("input", applyFilters);

renderTable(currentPage);
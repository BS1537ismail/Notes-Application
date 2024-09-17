let saveButton = document.getElementById('btnSave');
const deleteButton = document.getElementById('btnDelete');
const backButton = document.getElementById('btnBack');
const titleInput = document.getElementById('title');
const descriptionInput = document.getElementById('description');
const noteContainer = document.getElementById('notes_container');
const searchInput = document.getElementById('searchInput');
const searchButton = document.getElementById('btnSearch');



function clearForm() {
    btnSave.innerHTML = 'Save';
    titleInput.value = '';
    descriptionInput.value = '';
    deleteButton.classList.add('hidden');
    saveButton.removeAttribute('data-id');
    deleteButton.removeAttribute('data-id');
    currentPage = 1;
}

function displayNoteInForm(note) {
    titleInput.value = note.title;
    descriptionInput.value = note.description;
    deleteButton.classList.remove('hidden');
    deleteButton.setAttribute('data-id', note.id);
    saveButton.setAttribute('data-id', note.id);
}

backButton.addEventListener('click', function (event) {
    event.preventDefault();
    clearForm();
})

function getNoteById(id) {
    fetch(`https://localhost:7090/api/Notes/${id}`)
        .then(data => data.json())
        .then(response => displayNoteInForm(response));
}

function populateForm(id) {
    getNoteById(id);
}

let currentPage = 1;
let pageSize = 6;

//  displayNotes to include pagination data
function displayNotes(data) {
    let tempData = '';
    data.data.forEach(note => {
        const noteElement = `
            <div class="note" data-id="${note.id}">
                <h3>${note.title}</h3>
                <p>${note.description}</p>
            </div>
         `;
        tempData += noteElement;
    });
    noteContainer.innerHTML = tempData;

    // Update pagination UI
    let TotalPages = Math.ceil(data.totalRecords / data.pageSize);
    document.getElementById('pageNumber').innerText = `Page ${TotalPages} of ${data.pageNumber}`;
    document.getElementById('prevPage').disabled = data.pageNumber === 1;
    document.getElementById('nextPage').disabled = (data.pageNumber * data.pageSize) >= data.totalRecords;

    // Add event listeners to each note
    document.querySelectorAll('.note').forEach(note => {
        note.addEventListener('click', function () {
            saveButton.innerHTML = 'Update';
            const noteId = note.dataset.id;
            console.log(noteId);
            populateForm(noteId);
        });
    });
}

// Get notes with pagination
function getAllNotes(search = '', page = 1) {
    let url = `https://localhost:7090/api/Notes?pageNumber=${page}&pageSize=${pageSize}`;
    if (search) {
        url += `&search=${encodeURIComponent(search)}`;
    }

    fetch(url)
        .then(response => response.json())
        .then(data => {
            if (data.data) {
                displayNotes(data);
            } else {
                console.error("Unexpected API response format:", data);
            }
        })
        .catch(error => {
            console.error("Error fetching notes:", error);
        });
}

// Initial load
getAllNotes();

// Handle pagination button clicks
document.getElementById('prevPage').addEventListener('click', function () {
    if (currentPage > 1) {
        currentPage--;
        getAllNotes(searchInput.value.trim(), currentPage);
    }
});

document.getElementById('nextPage').addEventListener('click', function () {
    currentPage++;
    getAllNotes(searchInput.value.trim(), currentPage);
});

// Search button click
searchButton.addEventListener('click', function () {
    currentPage = 1;  // Reset to the first page on search
    const searchTerm = searchInput.value.trim();
    getAllNotes(searchTerm, currentPage);
});


saveButton.addEventListener('click', function () {
    const id = saveButton.dataset.id;
    if (id) {
        updateNote(id, titleInput.value, descriptionInput.value);
    } else {
        addNote(titleInput.value, descriptionInput.value);
    }
});

// Add Note
function addNote(title, description) {
    const body = {
        title: title,
        description: description,
        isVisible: true
    };

    fetch('https://localhost:7090/api/Notes', {
        method: 'POST',
        body: JSON.stringify(body),
        headers: {
            "content-type": "application/json"
        }
    })
        .then(data => data.json())
        .then(response => {
            clearForm();
            getAllNotes();
        });
}

// Update Note
function updateNote(id, title, description) {
    const body = {
        title: title,
        description: description,
        isVisible: true
    };

    fetch(`https://localhost:7090/api/Notes/${id}`, {
        method: 'PUT',
        body: JSON.stringify(body),
        headers: {
            "content-type": "application/json"
        }
    })
        .then(data => data.json())
        .then(response => {
            clearForm();
            getAllNotes();
        });
}

// Delete Note
function deleteNote(id) {
    fetch(`https://localhost:7090/api/Notes/${id}`, {
        method: 'DELETE',
        headers: {
            "content-type": "application/json"
        }
    })
        .then(response => {
            clearForm();
            getAllNotes();
        });
}

deleteButton.addEventListener('click', function () {
    const id = deleteButton.dataset.id;
    btnSave.innerHTML = 'Save'
    deleteNote(id);
});
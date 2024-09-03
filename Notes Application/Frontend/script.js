const saveButton = document.getElementById('btnSave');
const deleteButton = document.getElementById('btnDelete');

const titleInput = document.getElementById('title');
const descriptionInput = document.getElementById('description');
const noteContainer = document.getElementById('notes_container');


function clearForm(){
    titleInput.value = '';
    descriptionInput.value = '';
    deleteButton.classList.add('hidden');
}
function displayNoteInForm(note){
    titleInput.value = note.title;
    descriptionInput.value = note.description;
    deleteButton.classList.remove('hidden');
    deleteButton.setAttribute('data-id', note.id);
    saveButton.setAttribute('data-id', note.id);
}

function getNoteById(id){
    fetch(`https://localhost:7090/api/Notes/${id}`)
        .then(data => data.json())
        .then(response => displayNoteInForm(response));
}

function populateForm(id){
    getNoteById(id);
}
// Display notes
function displayNotes(notes) {
    let tempData = '';
    notes.forEach(note => {
        const noteElement = `
            <div class="note" data-id="${note.id}">
                <h3>${note.title}</h3>
                <p>${note.description}</p>
            </div>
        `;
        tempData += noteElement;
    });
    noteContainer.innerHTML = tempData;

    // Add event listeners to each note
    document.querySelectorAll('.note').forEach(note => {
        note.addEventListener('click', function() {
            const noteId = note.dataset.id;
            populateForm(noteId);
        });
    });
}

const notesPerPage = 8; 
let currentPage = 1;
let allNotes = []; 

function getAllNotes(page = 1) {
    fetch('https://localhost:7090/api/Notes')
        .then(response => response.json())
        .then(data => {
            if (Array.isArray(data)) {
                allNotes = data; 
                const totalNotes = allNotes.length; 
                displayPaginatedNotes(page, totalNotes);
                updatePagination(totalNotes);
            } else {
                console.error("Unexpected API response format:", data);
            }
        })
        .catch(error => {
            console.error("Error fetching notes:", error);
        });
}

function displayPaginatedNotes(page, totalNotes) {
    const startIndex = (page - 1) * notesPerPage;
    const paginatedNotes = allNotes.slice(startIndex, startIndex + notesPerPage);
    displayNotes(paginatedNotes);
}

function updatePagination(totalNotes) {
    const totalPages = Math.ceil(totalNotes / notesPerPage);
    const prevPageButton = document.getElementById('prevPage');
    const nextPageButton = document.getElementById('nextPage');
    const pageNumber = document.getElementById('pageNumber');

    pageNumber.textContent = `Page ${currentPage} of ${totalPages}`;

    prevPageButton.disabled = currentPage === 1;
    nextPageButton.disabled = currentPage === totalPages;

    console.log(`Total Notes: ${totalNotes}, Current Page: ${currentPage}, Total Pages: ${totalPages}`);
}

document.getElementById('prevPage').addEventListener('click', function() {
    if (currentPage > 1) {
        currentPage--;
        const totalNotes = allNotes.length;
        displayPaginatedNotes(currentPage, totalNotes);
        updatePagination(totalNotes);
    }
});

document.getElementById('nextPage').addEventListener('click', function() {
    const totalNotes = allNotes.length;
    const totalPages = Math.ceil(totalNotes / notesPerPage);
    if (currentPage < totalPages) {
        currentPage++;
        displayPaginatedNotes(currentPage, totalNotes);
        updatePagination(totalNotes);
    }
});

// Initial load
getAllNotes();


saveButton.addEventListener('click', function(){

    const id = saveButton.dataset.id;
    if(id){
        updateNote(id, titleInput.value, descriptionInput.value);
    }
    else {
        addNote(titleInput.value, descriptionInput.value);
    }
     
});
//add
function addNote(title, description){
    const body = {
        title: title,
        description: description,
        isVisible: true
    };
    
        fetch('https://localhost:7090/api/Notes',{
            method:'POST',
            body:JSON.stringify(body),
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

//update
function updateNote(id, title, description){
    const body = {
        title: title,
        description: description,
        isVisible: true
    };
    
        fetch(`https://localhost:7090/api/Notes/${id}`,{
            method:'PUT',
            body:JSON.stringify(body),
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
//delete
function deleteNote(id){
    fetch(`https://localhost:7090/api/Notes/${id}`,{
        method:'DELETE',
        headers: {
            "content-type": "application/json"
        }
    })
    .then(response => {
        clearForm();
        getAllNotes();
    });
}

deleteButton.addEventListener('click', function(){
    const id = deleteButton.dataset.id;
    deleteNote(id);
})



//search
const searchButton = document.getElementById('btnSearch');

searchButton.addEventListener('click', function() {
    const searchTerm = searchInput.value.toLowerCase();
    const notes = document.querySelectorAll('.note');

    notes.forEach(note => {
        const title = note.querySelector('h3').textContent.toLowerCase();
        if (title.includes(searchTerm)) {
            note.style.display = 'block';
        } else {
            note.style.display = 'none';
        }
    });
});

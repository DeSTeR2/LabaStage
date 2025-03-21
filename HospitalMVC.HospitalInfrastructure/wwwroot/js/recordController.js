let recordCount = 1;

function showReceiptModal(appointmentId) {
    document.getElementById('appointmentId').value = appointmentId;
    document.getElementById('receiptModal').style.display = 'block';
    toggleDollarSign('receiptAmount', 'span-icon');
}

function hideReceiptModal() {
    document.getElementById('receiptModal').style.display = 'none';
    document.getElementById('receiptForm').reset();
    const formCard = document.getElementById('formCard');
    while (formCard.children.length > 2) {
        formCard.removeChild(formCard.lastChild);
    }
    recordCount = 1;
    toggleDollarSign('receiptAmount', 'span-icon');
}

window.onclick = function (event) {
    var modal = document.getElementById('receiptModal');
    if (event.target == modal) {
        hideReceiptModal();
    }
}

function toggleDollarSign(inputId, spanId) {
    const input = document.getElementById(inputId);
    const dollarSign = document.getElementById(spanId);
    if (input && dollarSign) {
        if (input.value.length > 0) {
            dollarSign.style.opacity = '0';
            dollarSign.style.pointerEvents = 'none';
        } else {
            dollarSign.style.opacity = '1';
            dollarSign.style.pointerEvents = 'auto';
        }
    }
}

function addNewRecord() {
    const formCard = document.getElementById('formCard');

    const newAmountGroup = document.createElement('div');
    newAmountGroup.className = 'form-group';
    newAmountGroup.id = `record-${recordCount}`;
    newAmountGroup.innerHTML = `
            <label for="receiptAmount${recordCount}" class="form-label">Medical product name</label>
            <textarea class="form-control" id="receiptDescription${recordCount}"
                      name="description[${recordCount}]" rows="1" required></textarea>
            <button type="button" class="btn-remove" onclick="removeRecord(${recordCount})">×</button>
        `;

    const newDescGroup = document.createElement('div');
    newDescGroup.className = 'form-group';
    newDescGroup.id = `record-${recordCount}-desc`;
    newDescGroup.innerHTML = `
            <label for="receiptDescription${recordCount}" class="form-label">Description</label>
            <textarea class="form-control" id="receiptDescription${recordCount}"
                      name="description[${recordCount}]" rows="4" required></textarea>
        `;

    formCard.appendChild(newAmountGroup);
    formCard.appendChild(newDescGroup);

    const newInput = document.getElementById(`receiptAmount${recordCount}`);
    const newSpan = document.getElementById(`span-icon${recordCount}`);
    newInput.addEventListener('input', () => toggleDollarSign(`receiptAmount${recordCount}`, `span-icon${recordCount}`));
    newInput.addEventListener('focus', () => toggleDollarSign(`receiptAmount${recordCount}`, `span-icon${recordCount}`));
    newInput.addEventListener('blur', () => toggleDollarSign(`receiptAmount${recordCount}`, `span-icon${recordCount}`));

    recordCount++;
}

function removeRecord(index) {
    const amountGroup = document.getElementById(`record-${index}`);
    const descGroup = document.getElementById(`record-${index}-desc`);
    if (amountGroup && descGroup) {
        amountGroup.remove();
        descGroup.remove();
    }
}

function saveReceipt() {
    var names = []
    var descriptions = []
    const formCard = document.getElementById('formCard');

    var childNumber = 0;
    var child = formCard.firstChild;
    while (child && child.nodeType !== 1) {
        var value = child.child.child.value;
        if (childNumber % 2 == 0) {
            names.push(value);
        } else {
            descriptions.push(value);
        }

        childNumber++;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    const initialInput = document.getElementById('receiptAmount');
    if (initialInput) {
        initialInput.addEventListener('input', () => toggleDollarSign('receiptAmount', 'span-icon'));
        initialInput.addEventListener('focus', () => toggleDollarSign('receiptAmount', 'span-icon'));
        initialInput.addEventListener('blur', () => toggleDollarSign('receiptAmount', 'span-icon'));
    }
});

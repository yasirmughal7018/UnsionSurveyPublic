document.addEventListener('DOMContentLoaded', () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    // Function to display a message on the chat board
    function displayMessage(senderName, message, isFromAgent) {
        const chatBoard = document.getElementById('chat-board');
        const messageElement = document.createElement('div');
        messageElement.textContent = message; // Display only the message text
        console.log(senderName, isFromAgent);

        // Add the classes to the message element
        if (isFromAgent) {
            messageElement.classList.add('chatbox-message', 'from-agent');
        } else {
            messageElement.classList.add('chatbox-message', 'from-user');
        }
        chatBoard.appendChild(messageElement);
        chatBoard.scrollTop = chatBoard.scrollHeight; // Scroll to bottom
    }

    function displayInitialMessage(sender, message) {
        const chatBoard = document.getElementById('chat-board');
        const messageElement = document.createElement('div');
        messageElement.textContent = message;
        messageElement.className = `chatbox-message from-${sender}`;
        if (sender === 'agent' && !message.startsWith('Hello')) {
            messageElement.classList.add('clickable-message');
        }
        chatBoard.appendChild(messageElement);
        chatBoard.scrollTop = chatBoard.scrollHeight;
    }

    connection.on("ReceiveMessage", (senderName, message, isFromAgent) => {
        displayMessage(senderName, message, isFromAgent); // Only pass the message text to display
    });

    connection.on("ReceiveChatHistory", (messages) => {
        const chatBoard = document.getElementById("chat-board");
        chatBoard.innerHTML = ''; // Clear the chat board before loading history

        messages.forEach(({ userId, agentId, message, isFromAgent, userName, agentName }) => {
            const senderName = isFromAgent ? agentName : userName;
            displayMessage(senderName, message, isFromAgent);
        });

       // Scroll to bottom after loading history
    });

    // Start the connection
    connection.start().then(() => {
        console.log("Connected to SignalR hub");

        // Example of loading chat history (uncomment and modify if needed)
        // const userId = parseInt(document.getElementById('user-id').value);
        // if (isNaN(userId)) {
        //     console.error("Invalid userId");
        //     return;
        // }
        // connection.invoke("LoadChatHistory", userId, 0)
        //     .then(() => console.log("Chat history loaded successfully"))
        //     .catch(err => console.error("Failed to load chat history:", err.toString()));
    }).catch(err => console.error("SignalR connection failed:", err.toString()));

    // Handle message sending from user to agent
    document.getElementById('message-form').addEventListener('submit', (event) => {
        event.preventDefault(); // Prevent form submission

        const message = document.getElementById('message-input').value;
        const userId = document.getElementById('user-id').value;

        if (message && userId) {
            connection.invoke("SendMessageToAgent", userId, message)
                .then(() => {
                    document.getElementById('message-input').value = ''; // Clear input after sending
                })
                .catch(err => console.error("Failed to send message:", err.toString()));
        } else {
            console.error("Message or user ID is missing");
        }
    });

    // Handle initial message display when the chatbox is toggled open
    document.querySelector('.chatbox-toggle button').addEventListener('click', function () {
        document.querySelector('.chatbox').style.display = 'flex';
        document.querySelector('.chatbox-toggle').style.display = 'none';

        // Display default messages from the agent
        displayInitialMessage('agent', 'Hello! How can I assist you today?');
        displayInitialMessage('agent', 'Here are some common questions:');
        displayInitialMessage('agent', '1. What are your working hours?');
        displayInitialMessage('agent', '2. How can I track my order?');
    });

    document.querySelector('.chatbox-close').addEventListener('click', function () {
        document.querySelector('.chatbox').style.display = 'none';
        document.querySelector('.chatbox-toggle').style.display = 'block';
    });

    document.addEventListener('click', function (event) {
        if (event.target.classList.contains('clickable-message')) {
            sendMessage('user', event.target.textContent);
            respondToUserMessage(event.target.textContent);
        }
    });

    function sendMessage(sender, message = null) {
        const inputField = document.getElementById('message-input');
        const text = message || inputField.value.trim();
        if (text !== '') {
            const messageElement = document.createElement('div');
            messageElement.textContent = text;
            messageElement.className = `chatbox-message from-${sender}`;
            document.querySelector('.chatbox-messages').appendChild(messageElement);
            inputField.value = '';
            document.querySelector('.chatbox-body').scrollTop = document.querySelector('.chatbox-body').scrollHeight;
        }
    }

    function respondToUserMessage(userMessage = '') {
        let response = '';
        if (userMessage.includes('working hours')) {
            response = 'Our working hours are 9 AM to 5 PM from Monday to Friday.';
        } else if (userMessage.includes('track my order')) {
            response = 'You can track your order by visiting our Order Tracking page and entering your order ID.';
        } else {
            response = 'Sure, let me check that for you.';
        }
        setTimeout(function () {
            sendMessage('agent', response);
        }, 1000);
    }
});
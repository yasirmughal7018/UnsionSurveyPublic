document.addEventListener('DOMContentLoaded', () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    const agentName = document.getElementById('agent-name').value;
    const agentId = document.getElementById('agent-name').value;

    // Function to display a message on the chat board
    function displayMessage(senderName, message, isFromAgent) {
        const chatBoard = document.getElementById('chat-board');
        const messageElement = document.createElement('div');

        // Apply different styles for agent and user messages
        if (isFromAgent) {
            messageElement.classList.add('agent-message');
        } else {
            messageElement.classList.add('user-message');
        }

        messageElement.textContent = `${senderName}: ${message}`;
        chatBoard.appendChild(messageElement);
        chatBoard.scrollTop = chatBoard.scrollHeight; // Scroll to bottom
    }

    // Listen for chat history
    connection.on("ReceiveChatHistory", (messages) => {
        const chatBoard = document.getElementById('chat-board');
        chatBoard.innerHTML = ''; // Clear the chat board before loading history
        messages.forEach(({ userId, agentId, message, isFromAgent, userName, agentName }) => {
            const senderName = isFromAgent ? agentName : userName;
            displayMessage(senderName, message, isFromAgent);
        });
    });

    // Function to load user list
    function loadUserList() {
        fetch(`/ChatBoard/GetUserListForAgent?agentId=${agentId}`)
            .then(response => response.json())
            .then(users => {
                const userList = document.getElementById('users');
                userList.innerHTML = ''; // Clear the existing list
                users.forEach(user => {
                    const listItem = document.createElement('li');
                    listItem.setAttribute('data-user-id', user.userId);
                    listItem.textContent = user.username;
                    listItem.addEventListener('click', () => {
                        document.getElementById('user-id').value = user.userId;
                        document.getElementById('user-name').value = user.username;
                        document.getElementById('chat-header').textContent = user.username;
                        //var username = user.username;
                        $('chat-header').data('item', user.username);
                        connection.invoke("LoadChatHistory", user.userId, agentId)
                            .catch(err => console.error("Failed to load chat history:", err.toString()));
                    });
                    userList.appendChild(listItem);
                });

                // Optionally, load the chat history for the first user in the list on page load
                if (users.length > 0) {
                    userList.firstChild.click();
                }
            })
            .catch(err => console.error("Failed to load user list:", err.toString()));
    }

    // Start the SignalR connection
    connection.start().then(() => {
        console.log("Connected to SignalR hub");

        // Set agent status to available
        connection.invoke("SetAgentStatus", agentId, true)
            .catch(err => console.error("Failed to set agent status:", err.toString()));

        // Load the user list after connection starts
        loadUserList();

    }).catch(err => console.error("SignalR connection failed:", err.toString()));

    // Handle message sending from agent to user
    document.getElementById('message-form').addEventListener('submit', (event) => {
        event.preventDefault(); // Prevent default form submission

        const message = document.getElementById('message-input').value;
        const agentId = document.getElementById('agent-id').value;
        const userId = document.getElementById('user-id').value;

        if (message && agentId && userId) {
            connection.invoke("SendMessageToUser", agentId, userId, message)
                .then(() => {
                    document.getElementById('message-input').value = ''; // Clear input after sending

                    // Allow form submission to proceed after the message is sent
                    document.getElementById('message-form').submit();
                })
                .catch(err => console.error("Failed to send message:", err.toString()));
        } else {
            console.error("Message, agent ID, or user ID is missing");
        }
    });

    // Handle end chat button click
    document.getElementById('end-chat-button').addEventListener('click', () => {
        // Set agent status to available when the chat ends
        const agentId = document.getElementById('agent-id').value;
        connection.invoke("SetAgentStatus", agentId, true)
            .then(() => {
                console.log("Agent is now available");
            })
            .catch(err => console.error("Failed to set agent status:", err.toString()));
    });
});

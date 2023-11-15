# SignalR-MVC-Chat-App

## Demo

![](/wwwroot/demo.mkv)
## Overview

This repository contains a real-time messaging application implemented using SignalR. The application demonstrates bidirectional communication between clients and a server, enabling real-time updates and messaging.

## Steps for Implementation

### 1. Install SignalR NuGet Package

Add `Microsoft.AspNetCore.SignalR.Common` Package into project by using command

```bash
dotnet add package Microsoft.AspNetCore.SignalR.Common
```

### 2. Create a SignalR Hub

1. Add a new class to your project `ChatHub.cs`.
2. Inherit from `Hub` class in `Microsoft.AspNetCore.SignalR`.
3. Define methods for sending and receiving messages.

```csharp
public class ChatHub : Hub<IChatClient>
{
    private static Dictionary<string, string> _connections = new();

    // key -connectionID  value- username
    public async Task BroadCastMessage( string message)
    {
        var fromUserId = Context.ConnectionId;
        var fromUser = _connections[fromUserId];
        await Clients.All.ReceiveMessage(fromUser, message);
    }
}
```

Here In Above ,This method is invoked when a client sends a message, and it broadcasts the message to all connected clients.

### IChatClient Interface

The `IChatClient` interface defines the methods that can be invoked by the server and listened to by the SignalR clients. It serves as a contract for communication between the server and connected clients in the SignalR hub.

```csharp
namespace SignalRMVCApp.SignalRHub
{
    public interface IChatClient
    {
        Task NewUserJoined(string userName);
        Task ReceiveMessage(string fromUser, string message);
        Task GetConnectedUsers(List<string> users);
    }
}
```

### 3.Configure SignalR in Progarm.cs

```csharp
builder.Services.AddSignalR();
```

And after `builder.Build()` middleware pipeline should configured like this

```csharp
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapHub<ChatHub>("/notification-hub");
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
```

### 4. Creating view:

### I. Add Script referance for connection to SignalR Hub

```javascript
<script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/7.0.12/signalr.min.js"></script>
```

This is essential js file to setup Connection with Hub.

### II. SignalR Connection Setup

```html
<script>
  var connection = new signalR.HubConnectionBuilder()
    .withUrl("/notification-hub")
    .build();

  connection.on("NewUserJoined", function (username) {
    var newusername = document.getElementById("newuserjoined");
    newusername.innerText = username + " " + "joined chat ";
    setTimeout(() => {
      newusername.innerText = "";
    }, 3000);
  });

  connection.on("ReceiveMessage", function (fromUser, message) {
    var message = fromUser + " : " + message;
    var li = document.createElement("li");
    li.textContent = message;
    document.getElementById("chat").appendChild(li);
  });

  connection.on("GetConnectedUsers", function (connectedUsers) {
    var userList = document.getElementById("toUser");
    userList.innerHTML = "";
    connectedUsers.forEach(function (user) {
      var option = document.createElement("option");
      option.text = user;
      option.value = user;
      document.getElementById("toUser").add(option);
    });
  });

  connection.start().catch(function (err) {
    return console.error(err.toString());
  });
</script>
```

This script sets up a SignalR connection, defines event handlers for events raised by the server, and starts the connection. These are Function that we have defined in `IChatClient.cs` Interface.

### IV. User Interaction Functions

```javascript
<script>
function joinChat() {
    var username = document.getElementById("user").value;
    connection.invoke("JoinChat", username).catch(function (err) {
      return console.error(err.toString());
    });
  }

  function broadCastMessage() {
    var message = document.getElementById("message").value;
    connection.invoke("BroadCastMessage", message).catch(function (err) {
      return console.error(err.toString());
    });
  }

  function sendMessage() {
    var toUser = document.getElementById("toUser").value;
    var message = document.getElementById("message").value;

    // to show sended message
    var fromUser = document.getElementById("user").value;
    var fromMessage = fromUser + " : " + message;
    var li = document.createElement("li");
    li.textContent = fromMessage;
    document.getElementById("chat").appendChild(li);

    if (toUser == fromUser) {
      return;
    }
    connection
      .invoke("SendMessageToUser", toUser, message)
      .catch(function (err) {
        return console.error(err.toString());
      });
  }
  </script>
```

These functions are triggered by user interactions. joinChat is called when a user joins the chat, broadCastMessage broadcasts a message to all users, and sendMessage sends a message to a specific user.

### IV. HTML page

```html
<div class="text-center">
  <h5 id="newuserjoined" style="height: 20px;"></h5>
  <h1 class="display-4">SignalR MVC Chat App</h1>
  <input type="text" id="user" placeholder="Enter your name" />
  <button onclick="joinChat()">Join Chat</button>
  <br />
  <select id="toUser">
    <option value="">Select user to send message</option>
  </select>
  <br />
  <textarea id="message" placeholder="Type your message"></textarea>
  <br />
  <button onclick="sendMessage()">Send</button>
  <button onclick="broadCastMessage()">Broadcast to All</button>
  <hr />
  <div id="chat"></div>
</div>
```

These HTML elements create the user interface for the chat application, allowing users to input their name, send messages.

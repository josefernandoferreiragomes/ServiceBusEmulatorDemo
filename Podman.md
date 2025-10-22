## To use podman to run a container
Podman is a containerization tool similar to Docker. If you prefer to use Podman instead of Docker, you can follow these steps to run the Azure Service Bus Emulator using Podman.

### Prerequisites

1. Download the Installer
- Go to the official Podman site: https://podman.io/getting-started/installation
- Scroll to Windows and download the latest Podman Installer for Windows (.msi file).

2. Run the Installer
- Double-click the .msi file and follow the installation wizard.
- It will install:
- podman
- podman machine (for managing the VM)
- Optional: podman-compose

3. Initialize Podman Machine
Podman uses a virtual machine to run containers on Windows.
Run the following commands in PowerShell or Command Prompt:
```bash
podman machine init
podman machine start
```

This sets up and boots the Linux VM backend.
4. Verify Installation
Run the following commands in PowerShell or Command Prompt:
```bash
podman info
```

You should see system details confirming Podman is running.
5. Test with a Container
Try pulling and running a basic image:
```bash
podman run hello-world
```

This confirms Podman can pull and run containers.

🛠 Optional Enhancements
- Alias Docker to Podman (if you want to test Docker commands):
```bash
powershell New-Alias docker podman
```

- Use Podman Compose for multi-container setups:
```bash
podman compose -f docker-compose-simple.yaml build
podman compose -f docker-compose-simple.yaml up
```

Set PublisherDemo and SubscriberDemo as startup projects in your IDE to test sending and receiving messages.
Run solution


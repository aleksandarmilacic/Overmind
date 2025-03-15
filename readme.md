# 🚀 Overmind: AI Game Assistant

**Overmind** is an advanced AI-powered assistant designed to analyze game screenshots in real-time, providing expert strategy advice and guidance. Built specifically for complex strategy games like Stellaris, it leverages OpenAI's GPT-4 Vision to deliver detailed, actionable insights.

---

## 🛠️ Features

- **Real-time Screenshot Analysis:** Captures and analyzes game windows automatically.
- **Advanced AI Insights:** Delivers structured, strategic advice covering military, economy, technology, diplomacy, and more.
- **Text-to-Speech Integration:** Provides audible strategy suggestions through Windows built-in TTS.
- **Flexible and Configurable:** Supports various games beyond Stellaris through customizable window capture.

---

## 📦 Tech Stack

- .NET 9
- OpenAI GPT-4 Vision (GPT-4o)
- Windows API (user32.dll) for Window Capturing
- System.Drawing.Common
- Microsoft User Secrets for secure configuration

---

## 🚩 Getting Started

### 📌 Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [OpenAI API Key](https://platform.openai.com/api-keys)

### 🛠️ Installation

Clone the repository:

```bash
git clone https://github.com/your-username/overmind.git
cd overmind
```

Restore dependencies:

```bash
dotnet restore
```

Set up API Keys using User Secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "OPENAI_API_KEY" "your-openai-api-key"
```

### 🚀 Running the Application

Run the application from the command line:

```bash
dotnet run
```

---

## ⚙️ Customizing

- Change the target game window by editing the `GameName` variable in `Program.cs`.
- Adjust capture intervals and AI response settings directly within `Program.cs`.

---

## 📄 License

This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## 🤝 Contributing

Contributions are welcome! Feel free to fork this repository, create an issue, or submit a pull request.

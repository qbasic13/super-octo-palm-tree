const PROXY_CONFIG = [
  {
    context: [
      "/api",
      "/img"
    ],
    target: "https://localhost:40443",
    secure: false
  }
]

module.exports = PROXY_CONFIG;

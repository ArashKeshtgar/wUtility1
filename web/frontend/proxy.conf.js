// Dev-server proxy: forwards /api to the .NET API and attaches the API key
// on the server side, so it is never compiled into the browser bundle.
// Start with the same key the API uses:
//   WUTILITY_API_KEY=... npm start
const apiKey = process.env.WUTILITY_API_KEY;
if (!apiKey) {
  throw new Error('WUTILITY_API_KEY must be set before running ng serve (same value as the API).');
}

module.exports = {
  '/api': {
    target: 'http://localhost:5091',
    secure: false,
    changeOrigin: true,
    headers: { 'X-Api-Key': apiKey },
  },
};

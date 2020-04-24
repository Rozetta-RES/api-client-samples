const express = require('express');
const app = express();

app.use(express.json());



app.post('/api/callback', (req, res) => {
    console.log(`access-key:${req.headers['access-key']}`);
    console.log(`body:${JSON.stringify(req.body)}`);
    res.send("received");
});


const port = process.env.PORT || 3000;
app.listen(port, () => console.log(`Listening on port ${port}...`));

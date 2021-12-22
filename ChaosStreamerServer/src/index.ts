import express from 'express';
import fs from 'fs';
import * as __pkg from "./package.json"; // import for my debugging workflow
import * as config from "./config.json";
import * as __act from "./actions.json";

//this is excessively bad code. i didnt care about doing much of anything for this 
// aside from letting me remotely get and set shit, so enjoy whatever this is.
// if i wanted to make this expandable, id use, routers, but i didnt, so oh well.

const app = express();
var recentRequests = 0;
var actions: actionJSON = JSON.parse(fs.readFileSync('./actions.json', 'utf-8'))

setInterval(() => { actions = JSON.parse(fs.readFileSync('./actions.json', 'utf-8')) as actionJSON; })

function gotRequest(): void {
    recentRequests++;
    if (recentRequests > 3000 || JSON.stringify(actions).length > 10000) {
        if (recentRequests > 3000) throw new Error("Too many recent requests! WTF!");
        else throw new Error("JSON database is too large! WTF!");
    }
    setTimeout(() => {
        recentRequests--;
    }, 30 * 1000); // 30 sec
}

app.get('/', (req, res) => {
    res.send("why are you looking here\n\n<p>cant a guy just make a c&c server in peace</p>\n<p>this is my first time ever making a web server so uhh idk what im doing</p>\n\n<p>source code on github tho</p>");

});

var mix = fs.readFileSync('../assets/mix.jpg');
app.get('/assets/mix.jpg', (req, res) => {

    res.send(mix);
});

app.get('/gimme/:runOBS/:id/:name', (req, res, next) => {
    var streaming: boolean = parseInt(req.params.runOBS) == 1 ? true : false;
    if (streaming) console.log(`Got ID ${req.params.id} that is streaming. their data is '${actions[req.params.id] ?? "none"}' and their name is ${req.params.name}`);
    res.send(actions[req.params.id] ?? "none");


    if (actions[req.params.id]) actions[req.params.id] = undefined;
    fs.writeFileSync("./actions.json", JSON.stringify(actions, null, 2))
});

app.get('/admin/getall/:pass', (req, res) => {
    if (req.params.pass != config.adminPass) {
        console.log(`${req.ip} requested all actions with incorrect password ${req.params.pass}`);
        res.status(403).send("<b><h1>403: Forbidden</b></h1>\n<p>no dick</p>\n<p>no balls</p>\n<p>and <b>DEFINITELY</b> no butthole, since i KNOW you feed on radiation</p>");
        return;
    }
    res.status(200).send(JSON.stringify(actions, null, 2))
});

app.get('/admin/set/:pass/:id/:data', (req, res) => {
    if (req.params.pass != config.adminPass) {
        console.log(`${req.ip} requested setting data for "${req.params.id}" to "${req.params.data}" with incorrect password "${req.params.pass}"`);
        res.status(403).send('<b><h1>403: Forbidden</b></h1>\n<p>my child will mix at -6dB</p>\n<img src="../../../../assets/mix.jpg"/>');
        return;
    }
    console.log(`Set ${req.params.id} to ${req.params.data} as requested by ${req.ip}`);
    res.status(200).send(`from ${actions[req.params.id] ?? "none"} to ${req.params.data}`);
    actions[req.params.id] = req.params.data;
    fs.writeFileSync("./actions.json", JSON.stringify(actions, null, 2))
});

app.get('/admin/:pass/:id', (req, res) => {
    if (req.params.pass != config.adminPass) {
        console.log(`${req.ip} requested data for ${req.params.id} with incorrect password ${req.params.pass}`);
        res.status(403).send("<b><h1>403: Forbidden</b></h1>\nyour yeezy's are fake too");
        return;
    }
    console.log(`${req.ip} requested data for ${req.params.id} - data = ${actions[req.params.id]}`);

    res.status(200).send(actions[req.params.id] ?? "none");
});


app.listen(process.env.PORT ?? 5001, () => console.log('Listening! on ' + (process.env.PORT ?? 5001)))

interface actionJSON {
    [index: string]: string | undefined,
}
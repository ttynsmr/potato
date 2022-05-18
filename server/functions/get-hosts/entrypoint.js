var express = require("express");
var app = express();

var { listVMs } = require("./listVMs");

var server = app.listen(3000, function(){
  console.log("Node.js is listening to PORT:" + server.address().port);
});

//jq '.items."zones/asia-northeast1-b".instances[0].networkInterfaces[0].accessConfigs[0].natIP'

app.get("/", async (req, res) => {
  try {
    const result = await listVMs();
    console.log(JSON.stringify(result))

    if (result.items["zones/asia-northeast1-b"].instances[0].networkInterfaces[0].accessConfigs[0].natIP) {
      res.status(200).send(result.items["zones/asia-northeast1-b"].instances[0].networkInterfaces[0].accessConfigs[0].natIP);
    }
    else {
      res.status(404).send("");
    }
  } catch (error) {
    res.status(404).send("");
  }
});

exports.listVMs = listVMs;

'use strict';

var { listVMs } = require("./listVMs");

//jq '.items."zones/asia-northeast1-b".instances[0].networkInterfaces[0].accessConfigs[0].natIP'

exports.getHosts = async (req, res) => {
  try {
    const result = await listVMs();

    if (result.items["zones/asia-northeast1-b"].instances[0].networkInterfaces[0].accessConfigs[0].natIP) {
      res.status(200).send(result.items["zones/asia-northeast1-b"].instances[0].networkInterfaces[0].accessConfigs[0].natIP);
    }
    else {
      res.status(404).send("");
    }
  } catch (error) {
    res.status(404).send("");
  }
};

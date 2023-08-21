const express = require('express');
const axios = require('axios');
const cheerio = require('cheerio');
const https = require('http');

const app = express();

async function fetchLibraryStats() {
  try {    
    const response = await axios.get('https://library.mcmaster.ca/php/occupancy-spaces.php');
    const html = response.data;
    const $ = cheerio.load(html);
    const data = $('p.mt-2').map((index, element) => $(element).text()).get();
    const keyValue = {};
      
    data.forEach(line => {
      const [library, capacity] = line.split(' ');
      // const [library, capacity] = line.split(' at capacity ');
        keyValue[library] = capacity;
      });
      
    console.log(keyValue);
          
    const libraryStats = {      
      description: keyValue,      
      timestamp: new Date().toISOString(),
    };

    return libraryStats;

  } catch (error) {
    console.error('Error:', error);
    return null;
  }
}

app.get('/library-stats', async (req, res) => {
  console.log('Fetching library stats');
  const stats = await fetchLibraryStats();
  if (stats) {
    res.json(stats);
  } else {
    res.status(500).json({ error: 'Failed to fetch library stats' });
  }
});

app.listen(3000, () => {
  console.log('Server is running on port 3000');
});

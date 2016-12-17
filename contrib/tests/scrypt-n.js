// sample implementation: https://github.com/zone117x/node-stratum-pool/blob/master/lib/algoProperties.js#L53

var timeTable = {
  hash: function() {
  
    var timeTable = {
        "2048": 1389306217, 
        "4096": 1456415081, 
        "8192": 1506746729, 
        "16384": 1557078377, 
        "32768": 1657741673,
        "65536": 1859068265, 
        "131072": 2060394857, 
        "262144": 1722307603, 
        "524288": 1769642992
    };   
    
    console.log("timeTable");
    console.log(timeTable);
    
    var sorted = Object.keys(timeTable).sort();
    console.log("sorted");
    console.log(sorted);
    
    var reversed = sorted.reverse();
    console.log("reversed");
    console.log(reversed);
    
    var now = Date.now() / 1000;
    console.log("now: " + now);
    
    var filtered = reversed.filter(function(nKey) {
        return now > timeTable[nKey];
    });
    console.log("filtered");
    console.log(filtered); 
    
    var n = filtered[0];
    var nInt = parseInt(n);
    console.log("n=" + n);
    
    var nFactor = Math.log(nInt) / Math.log(2);
    console.log("nFactor=" + nFactor);
  } 
};

timeTable.hash();
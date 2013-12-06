var SMD = {
    "transport": "POST",
    "envelope": "URL",
    "target": "/json.rpc",
    "additonalParameters": false,
    "parameters": [],
    "types": {
        "0": {
            "__name": "string"
        },
        "1": {
            "__name": "boolean"
        },
        "2": {
            "__name": "int32"
        },
        "3": {
            "__name": "int64"
        },
        "4": {
            "__name": "object"
        },
        "5": {
            "__name": "smdadditionalparameters[]",
            "Length": 2,
            "LongLength": 3,
            "Rank": 2,
            "SyncRoot": 4,
            "IsReadOnly": 1,
            "IsFixedSize": 1,
            "IsSynchronized": 1
        },
        "6": {
            "__name": "list`1",
            "__genericArguments": [
        0
      ],
            "Capacity": 2,
            "Count": 2,
            "Item": 0
        },
        "7": {
            "__name": "dictionary`2"
        },
        "8": {
            "__name": "type"
        },
        "9": {
            "__name": "smdadditionalparameters",
            "ObjectType": 8,
            "Name": 0,
            "Type": 2,
            "Default": 4
        },
        "10": {
            "__name": "smdservice",
            "transport": 0,
            "envelope": 0,
            "additionalParameters": 9,
            "parameters": 5
        },
        "11": {
            "__name": "iequalitycomparer`1",
            "__genericArguments": [
        0
      ]
        },
        "12": {
            "__name": "keycollection",
            "__genericArguments": [
        0,
        10
      ],
            "Count": 2
        },
        "13": {
            "__name": "valuecollection",
            "__genericArguments": [
        0,
        10
      ],
            "Count": 2
        },
        "14": {
            "__name": "dictionary`2",
            "__genericArguments": [
        0,
        10
      ],
            "Comparer": 11,
            "Count": 2,
            "Keys": 12,
            "Values": 13,
            "Item": 10
        },
        "15": {
            "__name": "smd",
            "transport": 0,
            "envelope": 0,
            "target": 0,
            "additonalParameters": 1,
            "parameters": 5,
            "TypeHashes": 6,
            "Types": 7,
            "Services": 14
        },
        "16": {
            "__name": "single"
        },
        "17": {
            "__name": "customstring",
            "str": 0
        }
    },
    "services": {
        "internal.echo": {
            "transport": "POST",
            "envelope": "JSON-RPC-2.0",
            "additionalParameters": {
                "__name": "returns",
                "__type": 0,
                "__default": null
            },
            "parameters": [
        {
            "__name": "s",
            "__type": 0,
            "__default": null
        },
        null
      ]
        },
        "?": {
            "transport": "POST",
            "envelope": "JSON-RPC-2.0",
            "additionalParameters": {
                "__name": "returns",
                "__type": 15,
                "__default": null
            },
            "parameters": [
        null
      ]
        },
        "testFloat": {
            "transport": "POST",
            "envelope": "JSON-RPC-2.0",
            "additionalParameters": {
                "__name": "returns",
                "__type": 6,
                "__default": null
            },
            "parameters": [
        {
            "__name": "input",
            "__type": 16,
            "__default": null
        },
        null
      ]
        },
        "testInt": {
            "transport": "POST",
            "envelope": "JSON-RPC-2.0",
            "additionalParameters": {
                "__name": "returns",
                "__type": 6,
                "__default": null
            },
            "parameters": [
        {
            "__name": "input",
            "__type": 2,
            "__default": null
        },
        null
      ]
        },
        "testSimpleString": {
            "transport": "POST",
            "envelope": "JSON-RPC-2.0",
            "additionalParameters": {
                "__name": "returns",
                "__type": 6,
                "__default": null
            },
            "parameters": [
        {
            "__name": "input",
            "__type": 0,
            "__default": null
        },
        null
      ]
        },
        "testCustomString": {
            "transport": "POST",
            "envelope": "JSON-RPC-2.0",
            "additionalParameters": {
                "__name": "returns",
                "__type": 6,
                "__default": null
            },
            "parameters": [
        {
            "__name": "input",
            "__type": 17,
            "__default": null
        },
        null
      ]
        }
    }
};

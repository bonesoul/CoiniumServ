using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;


namespace CoiniumServ.Overpool.Stratum
{
    public static class Utilities
    {
        public static byte[] HexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format("The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] HexAsBytes = new byte[hexString.Length / 2];

            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                //HexAsBytes[index] = Convert.ToByte(byteValue);
                HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return HexAsBytes;
        }

        public static string ByteArrayToHexString(byte[] byteArray)
        {
            string result = "";

            foreach (byte b in byteArray)
                result += string.Format("{0:x2}", b);

            return result;
        }

        public static byte[] ReverseByteArrayByFours(byte[] byteArray)
        {
            byte temp;

            if (byteArray.Length % 4 != 0)
            {
                throw new ArgumentException(String.Format("The byte array length must be a multiple of 4"));
            }

            for (int index = 0; index < byteArray.Length; index += 4)
            {
                temp = byteArray[index];
                byteArray[index] = byteArray[index + 3];
                byteArray[index + 3] = byteArray[index + 2];
                byteArray[index + 2] = byteArray[index + 1];
                byteArray[index + 1] = byteArray[index + 3];
                byteArray[index + 3] = temp;
            }

            return byteArray;
        }

        // For testing:  Coinbase = Coinb1 + Extranonce1 + Extranonce2 + Coinb2
        //Coinb1 = "01000000010000000000000000000000000000000000000000000000000000000000000000ffffffff2703d72707062f503253482f049a53985208";
        //Coinb2 = "0d2f7374726174756d506f6f6c2f0000000001923f6f2e010000001976a9145b771921a9b47ee8104da7e4710b5f633d95fa7388ac00000000";
        //ExtraNonce1 = "f8025672";
        //ExtraNonce2 = "00000001";

        //MerkleNumbers = new string[5];
        //MerkleNumbers[0] = "a7f97d9ec804c539b375f2997bc12d32269d105a5281f2403c14fb1833b70f0c";
        //MerkleNumbers[1] = "04bd5a1f74d2beece0c893d742c51dd9b9f2e08e2686008d258f23461288a2cd";
        //MerkleNumbers[2] = "303ec9c3092b926f862ee2bd70b629e270e836eba86df9b55f9d795439163b13";
        //MerkleNumbers[3] = "2d43210a4fb228cf18001b630d0d45ca509b3db8ec3a40586b022546822b0dea";
        //MerkleNumbers[4] = "caf00c196faef37f97da33e8c6ef3cbc233f172a9a062d72f0dfd3ee17af291c";

        // Should generate a Merkle Root of 3c43e4bf024c2900181bd2f94bd7ebd5d3298f6d47f4f899b952acfd6aa6d94e
        
        public static string GenerateMerkleRoot(string Coinb1, string Coinb2, string ExtraNonce1, string ExtraNonce2, string[] MerkleNumbers)
        {
            string Coinbase = Coinb1 + ExtraNonce1 + ExtraNonce2 + Coinb2;

            byte[] Coinbasebytes = Utilities.HexStringToByteArray(Coinbase);

            SHA256 mySHA256 = SHA256.Create();
            mySHA256.Initialize();
            byte[] hashValue, hashValue2;

            // Create Coinbase hash by DoubleSHA of Coinbase
            hashValue = mySHA256.ComputeHash(Coinbasebytes);
            hashValue2 = mySHA256.ComputeHash(hashValue);

            // Calculate Merkle Root by double-hashing the Coinbase hash with each Merkle number in turn
            foreach (string s in MerkleNumbers)
            {
                hashValue = mySHA256.ComputeHash(Utilities.HexStringToByteArray(Utilities.ByteArrayToHexString(hashValue2) + s));
                hashValue2 = mySHA256.ComputeHash(hashValue);
            }

            string MerkleRoot = Utilities.ByteArrayToHexString(Utilities.ReverseByteArrayByFours(hashValue2));

            return MerkleRoot;
        }

        public static string GenerateTarget(int Difficulty)
        {
            // Calculate Target (which is the reverse of 0x 0000ffff 00000000 00000000 00000000 00000000 00000000 00000000 00000000 / difficulty
            byte[] ba = { 0x00, 0x00, 0xff, 0xff, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            int index = 0;
            int d = Difficulty;
            int n = ba[0];
            byte[] result = new byte[ba.Length];

            do
            {
                int r = n / d;
                result[index] = (byte)r;
                int x = n - r * d;

                if (++index == ba.Length)
                    break;

                n = (x << 8) + ba[index];
            } 
            while (true);

            Array.Reverse((Array)result);

            string Target = Utilities.ByteArrayToHexString(result);

            return Target;
        }
    
        /// <summary>        
        /// Serializes an object to a UTF-8 encoded JSON string.        
        /// </summary>        
        /// <param name="obj">object to serialize</param>        
        /// <returns>JSON string result</returns>        
        public static string JsonSerialize(object obj)
        {
            // Serialize to a memory stream....            
            MemoryStream ms = new MemoryStream();

            // Serialize to memory stream with DataContractJsonSerializer            
            DataContractJsonSerializer s = new DataContractJsonSerializer(obj.GetType());
            s.WriteObject(ms, obj);
            byte[] json = ms.ToArray();
            ms.Close();

            // Return utf8 encoded json string            
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        /// <summary>        
        /// Deserializes an object from a UTF-8 encoded JSON string.        
        /// </summary>        
        /// <typeparam name="T">type of object to deserialize as</typeparam>        
        /// <param name="json">UTF-8 encoded JSON string</param>        
        /// <returns>deserialized object</returns>

        public static T JsonDeserialize<T>(string json)
        {
            T result = default(T);
            
            // Load json into memorystream and deserialize
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));

            DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(T));
            result = (T)s.ReadObject(ms);
            ms.Close();
            return result;
        }
    }
}


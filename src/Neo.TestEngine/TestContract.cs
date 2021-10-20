// Copyright (C) 2015-2021 The Neo Project.
// 
// The Neo.Compiler.CSharp is free software distributed under the MIT 
// software license, see the accompanying file LICENSE in the main directory 
// of the project or http://www.opensource.org/licenses/mit-license.php 
// for more details.
// 
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.SmartContract;

namespace Neo.TestingEngine
{
    public class TestContract
    {
        internal string nefPath;
        internal object buildScript = null;

        public TestContract(string path)
        {
            nefPath = path;
        }
    }
}
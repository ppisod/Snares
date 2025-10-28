using System;

namespace ppilib.Erroring;

public class NodeConfigMissing(string attirbuteMissing, string classConstructing) : Exception(
    $"attribute {attirbuteMissing} is missing from NodeConfig when constructing {classConstructing}!");
using System;

namespace ppilib2.Graphics._Exceptions;

public class ChildAlreadyParented (string nameOfChildNode) : Exception($"node {nameOfChildNode} already has a parent");
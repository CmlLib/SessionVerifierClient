﻿namespace SessionVerifierClient;

public class VerifyException : Exception
{
    public VerifyException(string msg) : base(msg)
    {

    }
}
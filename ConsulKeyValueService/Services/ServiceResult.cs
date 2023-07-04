﻿namespace ConsulKeyValueService.Services
{
    public class ServiceResult <T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
    }
}

from kimola import Kimola  # Import the SDK class

# Initialize the SDK with your API key
sdk = Kimola(api_key="YOUR_API_KEY")

# Example: List available presets (currently commented out)
#print(sdk.presets.list())

# Get the current active subscription usage
current_usage = sdk.subscription.usage()
print("Current subscription usage:", current_usage)

# Close the SDK client gracefully
sdk.close()
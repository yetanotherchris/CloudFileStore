# Integration tests

These tests rely on appsettings.json containing valid credentials.

### Google

The most long winded of the 3...

1. Create a new project
1. Under IAM, click on "service accounts" on the left side.
1. Create a new service account, Storage Admin.
1. Download the JSON file.
1. Paste the JSON parts into appsettings.json under `"GoogleCloudConfiguration": {`, being careful not to delete the BucketName key.
1. Under "Storage" in the left menu, go to "Browser"
1. Create a bucket
1. Copy the bucket name into appsettings.json e.g. `"BucketName": "cloudfilestore-tests",`
1. Select the bucket in the browser, and under permissions on the right add the service account email as role "Storage Admin".

You can also store the JSON file that you downloaded from the Google console as `google.json`, which is used by the Github Actions workflow.

### Azure

1. Create a storage account with the defaults.
1. Once it's finished, navigate there and then click the "Security+Networking->Access Keys" link.
1. Copy the first connection string into appsettings.json.
1. Click "Data Storage->Containers" on the left
1. Click on the "+ Container" icon to create a container and call it "tests".
1. Copy the container name "tests" into the Azure appsettings.json section.

### AWS

1. Create an S3 bucket called `cloudfilestore-tests`.
1. Inside IAM, add a new service user, give them a custom policy called "cloudfilestore-tests" with this JSON:

```
{
    "Version": "2012-10-17",
    "Statement": [
        {
            "Effect": "Allow",
            "Action": "s3:*",
            "Resource": "arn:aws:s3:::cloudfilestore-tests"
        },
        {
            "Effect": "Allow",
            "Action": "s3:*",
            "Resource": "arn:aws:s3:::cloudfilestore-tests/*"
        }
    ]
}
```

Then generate an access/secret key for the user, add to appsettings.json

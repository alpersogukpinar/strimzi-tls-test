

# Ingress Installation for Kind 

Ingress required, ingress installation for kind Ingress NGINX
<br> https://kind.sigs.k8s.io/docs/user/ingress/#ingress-nginx
```
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/master/deploy/static/provider/kind/deploy.yaml
```
# Strimzi Installation
Installing Strimzi Kubernetes Cluster
1. Create a kafka namespace 
```
kubectl create namespace kafka
```
2. Create strimzi operator
```
kubectl apply -f https://strimzi.io/install/latest?namespace=kafka -n kafka
```
3. Goto  strimzi directy  and create kafka cluster
```
cd .\strimzi
kubectl apply -f kafka.yaml -n kafka
kubectl wait kafka/my-cluster --for=condition=Ready --timeout=300s -n kafka
```
4. Check kafka cluster
```
kubectl get kafka -n kafka
kubectl get kafka -o yaml -n kafka
```
5. Create user resource 
```
kubectl apply -f user.yaml -n kafka
```
6. Check secret
```
kubectl get secret my-user -n kafka
kubectl get secret my-user -o yaml -n kafka
```
7. Create a kafka topic
```
kubectl apply -f topic.yaml -n kafka
```
8. Create producer and consumer
```
kubectl apply -f hello-world.yaml -n kafka
kubectl get pods -n kafka
kubectl logs <consumer-pod-name> -f -n kafka
```
9. If you want you can delete consumer and producer deployments
```
kubectl delete deployment java-kafka-producer -n kafka
kubectl delete deployment java-kafka-consumer -n kafka
```
10. Publish message from .net client
```
kubectl port-forward my-cluster-kafka-0 9093:9093 -n kafka
```
11. I used kind kubernetes cluster and so set listeners' type **internal** type and added preceding lines to my hosts file, it shouldn't be necessary for loadbalancer or ingress case ...
```
127.0.0.1 my-cluster-kafka-0.my-cluster-kafka-brokers.kafka.svc
127.0.0.1 my-cluster-kafka-1.my-cluster-kafka-brokers.kafka.svc
127.0.0.1 my-cluster-kafka-2.my-cluster-kafka-brokers.kafka.svc
```
12.Read ca-cert with the following command, base64 decode and write it to **ca-root.crt** file
```
kubectl get secret my-cluster-cluster-ca-cert -o jsonpath='{.data.ca\.crt}' -n kafka
```
13.  Read my-user secret, Base64 decode **user.crt** data and save it to **user.crt** file.
Also, decode and save **user.key** data to **user.key** file
```
kubectl get secret my-user -o yaml -n kafka
```
14. Run .net console application publish message by writing to commandline somenthing and check consumer application logs with below commands
```
kubectl port-forward my-cluster-kafka-0 9093:9093 -n kafka
kubectl get pods -n kafka
kubectl logs <consumer-pod-name> -f -n kafka
```


# Lenses.io configuration for the Kafka Cluster
Documentation : <br>
- [Docker](https://docs.lenses.io/4.1/installation/docker/) <br>
- [Docker Quick Tour](https://docs.lenses.io/2.3/quick-tour/docker.html) <br>

Deployment Steps:
```
1. Create keystore : 
```
cd ../KafkaProducer/tmp/
openssl pkcs12 -export -inkey user.key -in user.crt -out kafka-client.keystore.p12 -name kafka-key <br>
keytool -importkeystore -destkeystore kafka-client.keystore.jks -deststorepass 123456 -srckeystore kafka.keystore.p12 -srcstoretype PKCS12 -srcstorepass 123456 
```
2. Create Truststore <br>
fetch password base64 decode 
kubectl get secret my-cluster-cluster-ca-cert -n kafka -o jsonpath='{.data.ca\.password}' <br>
keytool -importcert -alias KafkaCA -file ca.crt -keystore kafka-client.truststore.jks -keypass t6F2y1eMERve


helm install lenses lensesio/lenses -f lenses.yaml --namespace kafka  `
    --set lenses.licenseUrl=https://licenses.lenses.io/download/lensesdl?id=d5504c9b-8308-11eb-9025-42010af01003
    

helm install lenses lensesio/lenses --namespace kafka  `
    --set lenses.kafka=broker-0.myingress.com `
    --set lenses.opts.trustStoreFileData=MIIEVgIBAzCCBA8GCSqGSIb3DQEHAaCCBAAEggP8MIID+DCCA/QGCSqGSIb3DQEHBqCCA+UwggPhAgEAMIID2gYJKoZIhvcNAQcBMCkGCiqGSIb3DQEMAQYwGwQUWu2MpRlmOofd9Ybyw2cntobg8y8CAwDDUICCA6AL+y+/8Jp8qVNwtSxjPPDbrrRBt+jU3BHIYCY3Wy6cw3cE2BH/VYmcgkrOrNcBVYNWcwleEHi+KuyyR57maU9SQf1t1zFyjtX7qGjBk7tneF/r0hpIj1I5MNEGwSV5klJf9vQfAid8/puii6QqKt2ojRTwWBxk70EFE9+v1ep9WUYmwVh/Y7ByxooWywSDkOxL8+02oT//UEbF3Cn+MQp7dgZV82vkq5Wc4f8BDiOL2wrXUbK5G6TJGoyZoB2iCn3JM3J+9KkR4O+KWXuONa4zV9Z3ijERbBWQbIma1I9SqUhAgw7f/T6JYAq6yq8HvlBl3IDuN5Xbe5o1xZqv2HoGwZQbOO9ot9cRCngnuB6juUJk9tNFz2EZXdMRHMd3XX7PzP7fQVlMdtqf3XO2NA6EIqrzC24WcpF03sU8s7QNr3yHWJh7TR/y7ytD4qOjGTs2d9DWbL8xj1FrErXK7cXdRaSah/hrHz7CQ0YUyNGc7fsvTQP3VERiG2W/BEAN2P8u6feOnDiTSsQ2G/KUYJRpI6GnsHdapujLLa7wt3xO+DR1YAvSEdDqYapuBy7L+lcUPNAkMmwh2tP/l/tG6Q7s5Eazx3TrCCWRYqg4QePwtfQwLLVt2eUn5nnb2cks0mLUQcXlil79j3oKlA0NlN0Jg3NTD5GYFQT69kDneQ/WnxSgTwmO9YmmmIgnPSeH9KkiOPFcLa0ApG5ZuxUMDgYUgfK3nPwo5tGUT/ys1GuWVJ8oKGVhVpTQv8qSw0wKks8pHnZvD3jSlUTWx+bTuRRrjrzqGa25GTMCAyvmWKL4MVrhCNaVniQQH3Ucn0TSHlUn8SXvQ6Z8x8c2vAWJZHBj8Zw6hlurgskRrKRgjn0aWPMppKFe8zB0H80kTxAkCnsvEb2XrkV3QWmOhXhpq6/VBSHcai6JZUBW9v1/s3V0LsjLogA32XHzyVL/hq/ho1Ezrjr6PA1ID16yxI61b/iUvfCu8dAdGXdVMe4Yoq0DnE83cvsLlmeDasavRezDAp1yR7FYYKPiu+hTCuyD8NMxlQ5Q0kh1a1ofA3uZz6fc7+hihQVHM4E0F2IVFKK7UguC9Yxu9z9G2X6gzeoBbnyejyjBOR7sz6qqO/OSjEAPNbPDKbSS2u0Jkep05MM2D2BUksVHrA0gpElxqh26A0c51rxc4B+5A7ramJfpku8shl4b6NsyhP1xnW3XyRy5kSz6Rr6o6C9PHDimuiEOjU3SMD4wITAJBgUrDgMCGgUABBSmbt3kywoI9mFAOmTBJ/QrhMR9vwQUR4bSYT6N/BwYZKhwiUXe9gUCOwcCAwGGoA== `
    --set lenses.opts.keyStoreFileData=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 `
    --set lenses.opts.keyStorePassword=MTIzNDU2 --set lenses.opts.trustStorePassword=MTIzNDU2 `
    --set lenses.licenseUrl=https://licenses.lenses.io/download/lensesdl?id=d5504c9b-8308-11eb-9025-42010af01003
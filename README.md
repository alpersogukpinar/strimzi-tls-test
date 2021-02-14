# KafkaProducer
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
12.Read ca-cert with the following command and save it to **ca-root.crt** file
```
kubectl get secret my-cluster-cluster-ca-cert -o jsonpath='{.data.ca\.crt}' -n kafka
```
13. Read my-user secret, Base64 decode user.crt data and save it to **localhost_client.crt** file.
Also, decode and save user.key data to **localhost_client.key** file
```
kubectl get secret my-user -o yaml -n kafka
```

14. Run .net console application publish message by writing to commandline somenthing and check consumer application logs with below commands
```
kubectl port-forward my-cluster-kafka-0 9093:9093 -n kafka
kubectl get pods -n kafka
kubectl logs <consumer-pod-name> -f -n kafka
```
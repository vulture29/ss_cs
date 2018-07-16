# -*- coding: utf-8 -*-

import select
import socket
import json
import struct
import time
import threading
import random
from euclid import *
from map import *

HOST = ''
PORT = 5566
MAX_LISTEN_NUM = 10
USER_DB = './user_db'
zombunny_speed = 0.5
zombear_speed = 4
hellephant_speed = 2

init_data = {"Enemy": {"Hellephant": [], "Zombunny": [], "ZomBear": []}, "Item": {"Cake": [], "HealthPack": [], "Apple": [], "Pie": []}, "state": 1, "health": 100, "exp": 0, "Position": {"y": -1.484156e-05, "x": -3.581895, "z": -7.566027}}

class Connection:
    def __init__(self, sock, user="Guest"):
        self.sock = sock
        self.user = user
        self.logged_in = True


class Server:
    def __init__(self):
        self.host = HOST
        self.port = PORT
        self.connection_list = []
        self.db_lock = threading.Lock()
        self.init_db()
        self.init_server_socket()

    def init_db(self):
        self.user_dict = Server.load_dict(USER_DB)
        self.unique_id = self.user_dict["id"]

    @staticmethod
    def load_dict(file_path):
        try:
            with open(file_path) as f:
                load_dict = json.load(f)
            return load_dict
        except Exception as e:
            print(file_path + ' load fail: ' + str(e))
            return {}

    @staticmethod
    def write_dict(file_path, data_dict):
        try:
            with open(file_path, 'w') as f:
                json.dump(data_dict, f)
        except Exception as e:
            print(file_path + ' write fail: ' + str(e))
            return {}

    def handle_player_pos(self, user, msg_json):
        self.user_dict[user]["Position"] = msg_json["value"]

    def handle_enemy_health(self, user, msg_json):
        found = False
        enemy_dict = self.user_dict[user]["Enemy"]
        for enemy in enemy_dict["ZomBear"]:
            if str(enemy["id"]) == msg_json["id"]:
                found = True
                if int(msg_json["health"]) <= 0:
                    enemy_dict["ZomBear"].remove(enemy)
                else:
                    enemy["health"] = msg_json["health"]  
                    if self.user_dict[user]["state"] >= 2 and self.user_dict[user]["state"] < 3:
                        self.user_dict[user]["state"] += 0.2                  
        if not found:
            for enemy in enemy_dict["Zombunny"]:
                if str(enemy["id"]) == msg_json["id"]:
                    found = True
                    if int(msg_json["health"]) <= 0:
                        enemy_dict["Zombunny"].remove(enemy)
                        if self.user_dict[user]["state"] >= 1 and self.user_dict[user]["state"] < 2:
                            self.user_dict[user]["state"] += 0.1
                    else:
                        enemy["health"] = msg_json["health"]
        if not found:
            for enemy in enemy_dict["Hellephant"]:
                if str(enemy["id"]) == msg_json["id"]:
                    found = True
                    if int(msg_json["health"]) <= 0:
                        enemy_dict["Hellephant"].remove(enemy)
                        if self.user_dict[user]["state"] >= 3 and self.user_dict[user]["state"] < 4:
                            self.user_dict[user]["state"] += 0.5
                    else:
                        enemy["health"] = msg_json["health"]


    def handle_player_health(self, user, msg_json):
        if int(msg_json["health"]) <= 0:
            # player dead
            self.user_dict[user] = init_data
            print("Player dead")
        else:
            self.user_dict[user]["health"] = int(msg_json["health"])


    def handle_username_set(self, sock, msg_json):
        username = msg_json["value"]
        self.get_connection_from_sock(sock).user = username


    def handle_item_get(self, user, msg_json):
        item_id = int(msg_json["id"])
        found = False
        if not found:
            for cake in self.user_dict[user]["Item"]["Cake"]:
                if cake["id"] == item_id:
                    found = True
                    self.user_dict[user]["Item"]["Cake"].remove(cake)
                    break
        if not found:
            for pie in self.user_dict[user]["Item"]["Pie"]:
                if pie["id"] == item_id:
                    found = True
                    self.user_dict[user]["Item"]["Pie"].remove(pie)
        if not found:
            for apple in self.user_dict[user]["Item"]["Apple"]:
                if apple["id"] == item_id:
                    found = True
                    self.user_dict[user]["Item"]["Apple"].remove(apple)
        if not found:
            for health_pack in self.user_dict[user]["Item"]["HealthPack"]:
                if health_pack["id"] == item_id:
                    self.user_dict[user]["Item"]["HealthPack"].remove(health_pack)
                    self.user_dict[user]["health"] += 20
                    if self.user_dict[user]["health"] > 100:
                        self.user_dict[user]["health"] = 100

    def handle_cake_gen(self, user, msg_json):
        playerPos = self.user_dict[user]["Position"]
        cake_list = self.user_dict[user]["Item"]["Cake"]

        new_item = {"id": self.unique_id}
        self.unique_id += 1

        new_item["x"] = playerPos["x"]
        new_item["y"] = playerPos["y"]
        new_item["z"] = playerPos["z"]

        if len(cake_list) == 0:
            cake_list.append(new_item)

    def handle_change_level(self, user, msg_json):
        level = int(msg_json["value"])
        self.user_dict[user]["state"] = level


    def move_zombunny(self, user_position, enemy_list):
        # if len(enemy_list) > 0:
        #     print(str(enemy_list[0]))
        for enemy in enemy_list:
            dir_vector = nav_to_des(Point2(enemy["x"], enemy["z"]), Point2(user_position["x"], user_position["z"]))
            enemy["x"] = enemy["x"] + dir_vector.x * zombunny_speed
            enemy["z"] = enemy["z"] + dir_vector.z * zombunny_speed


    def move_zombear(self, user_position, enemy_list, item_list):
        pass
        for enemy in enemy_list:
            random_pos_x = 68 * random.random() - 34
            random_pos_z = 68 * random.random() - 34
            while abs(random_pos_x)+abs(random_pos_z) > 34 or (random_pos_x-user_position["x"])*(random_pos_x-user_position["x"])\
            +(random_pos_z-user_position["z"])*(random_pos_z-user_position["z"]) < 100:
                random_pos_z = 68 * random.random() - 34
            dir_vector = nav_to_des(Point2(enemy["x"], enemy["z"]), Point2(random_pos_x, random_pos_z))

            if len(item_list["Cake"]) > 0:
                dir_vector = nav_to_des(Point2(enemy["x"], enemy["z"]), Point2(item_list["Cake"][0]["x"], item_list["Cake"][0]["z"]))
                enemy["x"] = enemy["x"] + dir_vector.x * zombunny_speed
                enemy["z"] = enemy["z"] + dir_vector.z * zombunny_speed
            else:
                delta_x = user_position["x"] - enemy["x"]
                delta_z = user_position["z"] - enemy["z"]
                enemy["x"] = enemy["x"] + dir_vector.x * zombear_speed
                enemy["z"] = enemy["z"] + dir_vector.z * zombear_speed


    def move_hellephant(self, user_position, enemy_list):
        for enemy in enemy_list:
            dir_vector = nav_to_des(Point2(enemy["x"], enemy["z"]), Point2(user_position["x"], user_position["z"]))
            enemy["x"] = enemy["x"] + dir_vector.x * hellephant_speed
            enemy["z"] = enemy["z"] + dir_vector.z * zombunny_speed


    def update_enemy(self, user_dict):
        self.move_zombunny(user_dict["Position"], user_dict["Enemy"]["Zombunny"])
        self.move_zombear(user_dict["Position"], user_dict["Enemy"]["ZomBear"], user_dict["Item"])
        self.move_hellephant(user_dict["Position"], user_dict["Enemy"]["Hellephant"])
        

    def start_update_cron(self):
        # Enemy update
        threading.Timer(0.2, self.start_update_cron).start()
        for connection in self.connection_list:
            update_sock = connection.sock
            if update_sock != self.server_socket:
                self.db_lock.acquire()
                
                self.update_enemy(self.user_dict[connection.user])
                
                update_data = json.dumps(self.user_dict.get(connection.user, "{}"))
                self.db_lock.release()
                self.send_socket(update_sock, update_data)

    def start_io_cron(self):
        # I/O operation
        threading.Timer(2.0, self.start_io_cron).start()
        # print(str(len(self.connection_list)))

        self.db_lock.acquire()
        self.user_dict["id"] = self.unique_id
        tmp_dict = Server.load_dict(USER_DB)
        for key, value in tmp_dict.iteritems():
            if not key == "id" and key not in self.user_dict:
                self.user_dict[key] = value
        Server.write_dict(USER_DB, self.user_dict)
        self.db_lock.release()


    def start_enemy_generation_cron(self):
        # Enemy update
        threading.Timer(5, self.start_enemy_generation_cron).start()

        self.db_lock.acquire()
        # spawn enemy randomly
        for connection in self.connection_list:
            update_sock = connection.sock
            if update_sock != self.server_socket:
                playerPos = self.user_dict[connection.user]["Position"]

                # ZomBunny
                enemy_list = self.user_dict[connection.user]["Enemy"]["Zombunny"]
                if len(enemy_list) < 5:
                    new_enemy = {"health": 100, "id": self.unique_id}
                    self.unique_id += 1
                    x = 68 * random.random() - 34
                    z = 68 * random.random() - 34
                    while abs(x)+abs(z) > 34 or (x-playerPos["x"])*(x-playerPos["x"])+(z-playerPos["z"])*(z-playerPos["z"])<100:
                        z = 68 * random.random() - 34
                    new_enemy["x"] = x
                    new_enemy["y"] = 0
                    new_enemy["z"] = z
                    enemy_list.append(new_enemy)

                if self.user_dict[connection.user]["state"] > 2:
                    # ZomBear
                    enemy_list = self.user_dict[connection.user]["Enemy"]["ZomBear"]
                    if len(enemy_list) < 3 and not self.user_dict[connection.user]["state"] == 3:
                        new_enemy = {"health": 100, "id": self.unique_id}
                        self.unique_id += 1
                        x = 68 * random.random() - 34
                        z = 68 * random.random() - 34
                        while abs(x)+abs(z) > 34 or (x-playerPos["x"])*(x-playerPos["x"])+(z-playerPos["z"])*(z-playerPos["z"])<100:
                            z = 68 * random.random() - 34
                        new_enemy["x"] = x
                        new_enemy["y"] = 0
                        new_enemy["z"] = z
                        enemy_list.append(new_enemy)

                if self.user_dict[connection.user]["state"] >= 3:
                    # Hellephant
                    enemy_list = self.user_dict[connection.user]["Enemy"]["Hellephant"]
                    if len(enemy_list) < 1:
                        new_enemy = {"health": 100, "id": self.unique_id}
                        self.unique_id += 1
                        x = 68 * random.random() - 34
                        z = 68 * random.random() - 34
                        while abs(x)+abs(z) > 34 or (x-playerPos["x"])*(x-playerPos["x"])+(z-playerPos["z"])*(z-playerPos["z"])<100:
                            z = 68 * random.random() - 34
                        new_enemy["x"] = x
                        new_enemy["y"] = 0
                        new_enemy["z"] = z
                        enemy_list.append(new_enemy)

        self.db_lock.release()


    def start_item_generation_cron(self):
        # Enemy update
        threading.Timer(1, self.start_item_generation_cron).start()

        self.db_lock.acquire()
        # spawn enemy randomly
        for connection in self.connection_list:
            update_sock = connection.sock
            if update_sock != self.server_socket:
                playerPos = self.user_dict[connection.user]["Position"]
                item_list = self.user_dict[connection.user]["Item"]

                new_item = {"id": self.unique_id}
                self.unique_id += 1
                x = 68 * random.random() - 34
                z = 68 * random.random() - 34
                while abs(x)+abs(z) > 34 or (x-playerPos["x"])*(x-playerPos["x"])+(z-playerPos["z"])*(z-playerPos["z"])<100:
                    z = 68 * random.random() - 34
                new_item["x"] = x
                new_item["y"] = 0
                new_item["z"] = z

                if len(item_list["HealthPack"]) == 0 and random.random() < 0.33:
                    item_list["HealthPack"].append(new_item)
                elif len(item_list["Pie"]) == 0 and len(item_list["Cake"]) == 0 and random.random() < 0.5:
                    item_list["Pie"].append(new_item)
                elif len(item_list["Apple"]) == 0 and len(item_list["Cake"]) == 0:
                    item_list["Apple"].append(new_item)

        self.db_lock.release()

    def init_server_socket(self):
        self.server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)  # read it
        self.server_socket.bind((self.host, self.port))
        self.server_socket.listen(MAX_LISTEN_NUM)

    def get_connection_from_sock(self, sock):
        for connection in self.connection_list:
            if connection.sock == sock:
                return connection

    def get_user_from_sock(self, sock):
        for connection in self.connection_list:
            if connection.sock == sock:
                return connection.user
        return None

    def get_sock_from_user(self, user):
        for connection in self.connection_list:
            if connection.user == user:
                return connection.sock
        return None

    def handle_new_connection(self):
        sock, sock_addr = self.server_socket.accept()
        connection = Connection(sock)
        self.connection_list.append(connection)
        print("Client (%s, %s) connected" % sock_addr)
        
        init_data = json.dumps(self.user_dict.get("Guest", "{}"))
        print("Init data: " + init_data)

        self.send_socket(sock, init_data)

    def handle_client_msg(self, sock):
        try:
            # print("handle client msg")
            msg_len_str = sock.recv(4)
            msg_len = struct.unpack("!i", msg_len_str)[0]
            print("Recieve data length: " + str(msg_len))

            msg_str = sock.recv(msg_len)
            print("Recieve data: " + str(msg_str))

            msg_json = json.loads(msg_str)
            user = self.get_user_from_sock(sock)

            # self.db_lock.acquire()
            
            if msg_json["type"] == "playerPos":
                self.handle_player_pos(user, msg_json)
            elif msg_json["type"] == "enemyHealth":
                self.handle_enemy_health(user, msg_json)
            elif msg_json["type"] == "playerHealth":
                self.handle_player_health(user, msg_json)
            elif msg_json["type"] == "itemGet":
                self.handle_item_get(user, msg_json)
            elif msg_json["type"] == "cakeGen":
                self.handle_cake_gen(user, msg_json)
            elif msg_json["type"] == "level":
                self.handle_change_level(user, msg_json)
            elif msg_json["type"] == "playerUsername":
                print("playerUsername")
                self.handle_username_set(sock, msg_json)

            
            # self.db_lock.release()

        except Exception as e:
            print("receive Exception: " + str(e))
            sock.close()
            self.handle_disconnect(sock)

    def send_socket(self, sock, message):
        try:
            msg_len_str = struct.pack('i', len(message))
            sock.send(msg_len_str)
            while len(message) > 1024:
                sock.send(message[:1024])
                # print("sent: " + str(message[:1024]))
                message = message[1024:]
            sock.send(message)
            # print("sent: " + str(message))
        except:
            # broken socket, remove it
            print("send Exception")
            # broken socket connection
            sock.close()
            self.handle_disconnect(sock)

    def handle_disconnect(self, sock, data=None):
        for connection in self.connection_list:
            self.remove_sock(sock)
            print("Client disconnected")

    def remove_sock(self, sock):
        for connection in self.connection_list:
            if connection.sock == sock:
                self.connection_list.remove(connection)

    def start_server(self):
        self.connection_list.append(Connection(self.server_socket))
        print("Server started on port " + str(PORT))

        self.start_update_cron()
        self.start_io_cron()
        self.start_enemy_generation_cron()
        self.start_item_generation_cron()

        while True:
            ready_to_read, ready_to_write, in_error = select.select(
                [connection.sock for connection in self.connection_list], [], [], 0)

            for sock in ready_to_read:
                # new connection request
                if sock == self.server_socket:
                    # print("new connection...")
                    self.handle_new_connection()
                # msg from an existing connection
                else:
                    # print("existing connection...")
                    self.handle_client_msg(sock)
        server_socket.close()

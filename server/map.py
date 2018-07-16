from euclid import *
from graph import *
import math

map_obs_info = [\
[Point2(-3.24, 25.54), Point2(2.5, 31.65), Point2(10.8, 23.59), Point2(5.5, 17.82)],\
[Point2(6.1, 10.47), Point2(10.83, 7.02), Point2(10, 5.62), Point2(5.1, 8.99)],\
[Point2(18.84, 6.2), Point2(20.9, 6.2), Point2(20.9, 0.9), Point2(18.84, 0.9)],\
[Point2(-9.01, 10.22), Point2(-11.6, 13.73), Point2(-8.2, 16.4), Point2(-5.9, 12.76)],\
[Point2(-17.46, 0), Point2(-14.9, 0.2), Point2(-14.06, -2.4), Point2(-17.25, -4.1)],\
[Point2(-23.13, 5.1), Point2(-26.39, 8.6), Point2(-22.4, 12.7), Point2(-19.09, 9.4)]\
]
way_points = [Point2(-7.49, 24.8), Point2(-17.77, 13.83), Point2(-24.7, -2.8), Point2(15.27, -10.8), \
Point2(-12.8, 7.7), Point2(2.55, 11.1), Point2(-3.57, -12.08), Point2(19.14, 11.4)]
map_graph = [[0, 15.033938273120587, 32.52605263477264, 42.253728829536456, float('inf'), 16.985040476843146, float('inf'), float('inf')], [15.033938273120587, 0, float('inf'), 41.21017471450467, 7.89162847579636, float('inf'), 29.5460335747457, float('inf')], [32.52605263477264, float('inf'), 0, 40.76273911306746, 15.870097668256486, 30.590398820544983, 23.078026345422174, float('inf')], [42.253728829536456, 41.21017471450467, 40.76273911306746, 0, 33.618074007890456, 25.32604193315647, 18.88343189147566, 22.534793098672996], [float('inf'), 7.89162847579636, 15.870097668256486, 33.618074007890456, 0, 15.722038671877131, 21.827535362472787, float('inf')], [16.985040476843146, float('inf'), 30.590398820544983, 25.32604193315647, 15.722038671877131, 0, 23.974294567306877, 16.592712255686227], [float('inf'), 29.5460335747457, 23.078026345422174, 18.88343189147566, 21.827535362472787, 23.974294567306877, 0, 32.665800158575635], [float('inf'), float('inf'), float('inf'), 22.534793098672996, float('inf'), 16.592712255686227, 32.665800158575635, 0]]
map_dist_list = [{0: 0, 1: 15.033938273120587, 2: 32.52605263477264, 3: 42.253728829536456, 4: 22.925566748916946, 5: 16.985040476843146, 6: 40.95933504415002, 7: 33.577752732529376}, {0: 15.033938273120587, 1: 0, 2: 23.761726144052847, 3: 41.21017471450467, 4: 7.89162847579636, 5: 23.613667147673493, 6: 29.5460335747457, 7: 40.20637940335972}, {0: 32.52605263477264, 1: 23.761726144052847, 2: 0, 3: 40.76273911306746, 4: 15.870097668256486, 5: 30.590398820544983, 6: 23.078026345422174, 7: 47.183111076231214}, {0: 42.253728829536456, 1: 41.21017471450467, 2: 40.76273911306746, 3: 0, 4: 33.618074007890456, 5: 25.32604193315647, 6: 18.88343189147566, 7: 22.534793098672996}, {0: 22.925566748916946, 1: 7.89162847579636, 2: 15.870097668256486, 3: 33.618074007890456, 4: 0, 5: 15.722038671877131, 6: 21.827535362472787, 7: 32.31475092756336}, {0: 16.985040476843146, 1: 23.613667147673493, 2: 30.590398820544983, 3: 25.32604193315647, 4: 15.722038671877131, 5: 0, 6: 23.974294567306877, 7: 16.592712255686227}, {0: 40.95933504415002, 1: 29.5460335747457, 2: 23.078026345422174, 3: 18.88343189147566, 4: 21.827535362472787, 5: 23.974294567306877, 6: 0, 7: 32.665800158575635}, {0: 33.577752732529376, 1: 40.20637940335972, 2: 47.183111076231214, 3: 22.534793098672996, 4: 32.31475092756336, 5: 16.592712255686227, 6: 32.665800158575635, 7: 0}]
map_path_list = [{1: 0, 2: 0, 3: 0, 4: 1, 5: 0, 6: 5, 7: 5}, {0: 1, 2: 4, 3: 1, 4: 1, 5: 4, 6: 1, 7: 5}, {0: 2, 1: 4, 3: 2, 4: 2, 5: 2, 6: 2, 7: 5}, {0: 3, 1: 3, 2: 3, 4: 3, 5: 3, 6: 3, 7: 3}, {0: 1, 1: 4, 2: 4, 3: 4, 5: 4, 6: 4, 7: 5}, {0: 5, 1: 4, 2: 5, 3: 5, 4: 5, 6: 5, 7: 5}, {0: 5, 1: 6, 2: 6, 3: 6, 4: 6, 5: 6, 7: 6}, {0: 5, 1: 4, 2: 5, 3: 7, 4: 5, 5: 7, 6: 7}]

def is_valid_point(p):
    for rec in map_obs_info:
        a = rec[0].distance(rec[1])
        b = rec[1].distance(rec[2])
        c = rec[0].distance(rec[2])
        s = (a + b + c) / 2
        s1 = math.sqrt(s*(s-a)*(s-b)*(s-c))
        a = rec[0].distance(rec[3])
        b = rec[3].distance(rec[2])
        c = rec[0].distance(rec[2])
        s = (a + b + c) / 2
        s2 = math.sqrt(s*(s-a)*(s-b)*(s-c))
        s_rect = s1 + s2

        a = rec[0].distance(rec[1])
        b = rec[0].distance(p)
        c = rec[1].distance(p)
        s = (a + b + c) / 2
        st0 = math.sqrt(s*(s-a)*(s-b)*(s-c))
        a = rec[0].distance(rec[3])
        b = rec[0].distance(p)
        c = rec[3].distance(p)
        s = (a + b + c) / 2
        st1 = math.sqrt(s*(s-a)*(s-b)*(s-c))
        a = rec[2].distance(rec[1])
        b = rec[2].distance(p)
        c = rec[1].distance(p)
        s = (a + b + c) / 2
        st2 = math.sqrt(s*(s-a)*(s-b)*(s-c))
        a = rec[2].distance(rec[3])
        b = rec[2].distance(p)
        c = rec[3].distance(p)
        s = (a + b + c) / 2
        st3 = math.sqrt(s*(s-a)*(s-b)*(s-c))
        print(str(abs(st0+st1+st2+st3-s_rect)))
        if abs(st0+st1+st2+st3-s_rect) < 0.1:
            return False
    return True


def can_watch_point(ori, des):
    watch_line = LineSegment2(ori, des)
    for rec in map_obs_info:
        l0 = LineSegment2(rec[0], rec[1])
        l1 = LineSegment2(rec[1], rec[2])
        l2 = LineSegment2(rec[2], rec[3])
        l3 = LineSegment2(rec[3], rec[0])
        if l0.intersect(watch_line) is not None:
            return False
        if l1.intersect(watch_line) is not None:
            return False
        if l2.intersect(watch_line) is not None:
            return False
        if l3.intersect(watch_line) is not None:
            return False
    return True


def get_waypoint_index(p):
    for index, way_point in enumerate(way_points):
        if p == way_point:
            return index
    return -1


def nav_to_des(ori, des):
    if can_watch_point(ori, des):
        delta_x = des.x - ori.x
        delta_y = des.y - ori.y
        dir_vector = Vector3(delta_x, 0, delta_y)
        dir_vector.normalize()
        return dir_vector

    ori_watchable_waypoints = [way_point for way_point in way_points if can_watch_point(ori, way_point)]
    des_watchable_waypoints = [way_point for way_point in way_points if can_watch_point(des, way_point)]
    min_dist = float('inf')
    next_waypoint = Point2(0, 0)
    for ori_watchable_waypoint in ori_watchable_waypoints:
        for des_watchable_waypoint in des_watchable_waypoints:
            ori_index = get_waypoint_index(ori_watchable_waypoint)
            des_index = get_waypoint_index(des_watchable_waypoint)
            dist_tmp = ori.distance(ori_watchable_waypoint) + des.distance(des_watchable_waypoint)\
            + map_dist_list[ori_index][des_index]
            if dist_tmp < min_dist:
                min_dist = dist_tmp
                next_waypoint = ori_watchable_waypoint
    delta_x = next_waypoint.x - ori.x
    delta_y = next_waypoint.y - ori.y
    dir_vector = Vector3(delta_x, 0, delta_y)
    dir_vector.normalize()
    return dir_vector


def generate_graph():
    graph = []
    for outter_point in way_points:
        graph_list = []
        for inner_point in way_points:
            if outter_point == inner_point:
                graph_list.append(0)
            elif can_watch_point(inner_point, outter_point):
                graph_list.append(outter_point.distance(inner_point))
            else:
                graph_list.append(float('inf'))
        graph.append(graph_list)
    return graph


def generate_shortest_path():
    graph = Graph()

    for i in range(0,8):
        graph.add_node(i)

    for i in range(0,8):
        graph.add_node(i)

    for i in range(0,8):
        for j in range(0, 8):
            graph.add_edge(i, j, map_graph[i][j])

    for i in range(0,8):
        dist, path = dijsktra(graph, i)
        map_dist_list.append(dist)
        map_path_list.append(path)

    return map_dist_list, map_path_list